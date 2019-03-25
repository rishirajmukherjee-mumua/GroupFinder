using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupFinder.Models
{
    public class Questions
    {
        public ClassMate classmate { get; set; }

        public List<Vacation> vacations { get; set; }

        public List<IdealSaturday> idealSaturdays { get; set; }

        public List<Food> foods { get; set; }

        public string songorartist;

        [System.ComponentModel.DataAnnotations.Key]
        public int QuestionId { get; set; }

    }
}