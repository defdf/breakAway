﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BreakAway.Models.Address
{
    public class AddressViewModel
    {
        public int Id { get; set; }

        public Mail Mail { get; set; }

        public string CountryRegion { get; set; }

        public string PostalCode { get; set; }

        public string AddressType { get; set; }

        //public int ContactId { get; set; }

    }

    public class Mail
    {
        public string Street1 { get; set; }

        public string Street2 { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }
    }
}