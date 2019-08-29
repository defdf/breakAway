﻿using System;
using System.Linq;
using System.Web.Mvc;
using BreakAway.Entities;
using BreakAway.Models.Contact;

namespace BreakAway.Controllers
{
    public class ContactController : Controller
    {
        private readonly Repository _repository;

        public ContactController(Repository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            _repository = repository;
        }

        public ActionResult Index(string firstNameFilter, string lastNameFilter)
        {
            var viewModel = new Models.Contact.IndexViewModel();

            var contacts = from contact in _repository.Contacts
                           select contact;

            if (!string.IsNullOrWhiteSpace(firstNameFilter))
            {
                contacts = contacts.Where(c => c.FirstName.Contains(firstNameFilter));
            }
            if (!string.IsNullOrWhiteSpace(lastNameFilter))
            {
                contacts = contacts.Where(c => c.LastName.Contains(lastNameFilter));
            }
            viewModel.Contacts = (from contact in contacts
                                  select new ContactItem
                                  {
                                      Id = contact.Id,
                                      FirstName = contact.FirstName,
                                      LastName = contact.LastName,
                                      Title = contact.Title
                                  }).ToArray();

            return View(viewModel);
        }

        public ActionResult Add()
        {
            var viewModel = new AddViewModel { };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Add", "Contact", new { message = "Contact not created" });
            }

            var contact = new Contact
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Title = model.Title,
                AddDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
            _repository.Contacts.Add(contact);
            _repository.Save();

            return RedirectToAction("Index", "Customer", new { message = "Contact created successfully" });
        }

        public ActionResult Edit(int id)
        {

            var contact = _repository.Contacts.FirstOrDefault(c => c.Id == id);

            if (contact == null)
            {
                return RedirectToAction("Index", "Contact");
            }

            var viewModel = new EditViewModel
            {
                Id = contact.Id,
                Title = contact.Title,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Addresses = contact.Addresses.Select(address => new AddressViewModel
                {
                    Id = address.Id,
                    AddressType = address.AddressType,
                    Mail = new MailModel
                    {
                        Street1 = address.Mail.Street1,
                        Street2 = address.Mail.Street2,
                        City = address.Mail.City,
                        StateProvince = address.Mail.StateProvince
                    },
                    PostalCode = address.PostalCode,
                    CountryRegion = address.CountryRegion
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var contact = _repository.Contacts.FirstOrDefault(c => c.Id == model.Id);
            if (contact == null)
            {
                return RedirectToAction("Index", "Contact", new { message = "Contact with id '" + model.Id + "' was not found" });
            }

            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.Title = model.Title;

            foreach (var item in model.Addresses)
            {
                var address = contact.Addresses.SingleOrDefault(a => a.Id == item.Id);
                if (address == null) continue;
                address.AddressType = item.AddressType;
                address.PostalCode = item.PostalCode;
                address.CountryRegion = item.CountryRegion;
                address.Mail.Street1 = item.Mail.Street1;
                address.Mail.Street2 = item.Mail.Street2;
                address.Mail.City = item.Mail.City;
                address.Mail.StateProvince = item.Mail.StateProvince;
            }

            _repository.Save();

            return RedirectToAction("Index", "Contact", new { id = contact.Id, message = "Changes saved successfully" });
        }
    }
}