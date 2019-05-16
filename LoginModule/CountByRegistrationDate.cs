using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoginModule.Model;
using Raven.Client.Documents.Indexes;

namespace LoginModule
{
    public class CountByRegistrationDate : 
        AbstractIndexCreationTask<User, CountByRegistrationDate.Result>
    {
        public class Result
        {
            public int Count { get; set; }
            public DateTime RegistrationDate { get; set; }
        }

        public CountByRegistrationDate()
        {
            Map = users => 
                from user in users
                select new
                {
                    Count = 1,
                    RegistrationDate = user.RegisteredAt.Date
                };
            Reduce = results =>
                from result in results
                group result by result.RegistrationDate into g
                select new
                {
                    Count = g.Sum(x => x.Count),
                    RegistrationDate = g.Key
                };
        }
    }
}
