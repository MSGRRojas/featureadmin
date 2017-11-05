﻿using FeatureAdmin.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureAdmin.Core.Messages
{
    public class LocationsUpdated
    {
        public LocationsUpdated(IEnumerable<Location> spLocations)
        {
            Locations = Locations;
        }
        public IEnumerable<Location> Locations { get; private set; }
    }
}
