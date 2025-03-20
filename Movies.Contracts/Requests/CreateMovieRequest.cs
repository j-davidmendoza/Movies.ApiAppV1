﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Contracts.Requests
{
    internal class CreateMovieRequest
    {
        public required string Tittle {  get; init; }
        public required int YearOfRelease { get; init; }
        public required IEnumerable<string> Genres { get; init; }  = Enumerable.Empty<string>();    
    }
}
