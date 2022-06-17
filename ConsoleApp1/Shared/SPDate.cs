using System;

namespace SPLib.Shared
{
    public class SPDate
    {
        private string _stringValue;

        public string StringValue { 
            get 
            {
                return _stringValue;
            } 
            set 
            {
                _stringValue = value;

                DateTime result;

                if (DateTime.TryParse(value, out result))
                    this.DateValue = result;
            }
        }

        public DateTime DateValue { get; set; }

        public SPDate()
        {

        }

        public SPDate(SPDate input)
        {
            this.StringValue = input.StringValue;
            this.DateValue = input.DateValue;
        }

        public SPDate(string input)
        {
            DateTime result;

            if (DateTime.TryParse(input, out result))
                this.DateValue = result;
        }

        public SPDate(DateTime input)
        {
            this.DateValue = input;
            this.StringValue = input.ToString("yyyy-MM-dd");
        }
    }
}
