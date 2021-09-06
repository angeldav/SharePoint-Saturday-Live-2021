using System.ComponentModel.DataAnnotations;

namespace O365.ActivityManagementAPI.Entities
{
    public class O365Subscription
    {
        [Display(Name = "ContentUri")]
        public string ContentUri { get; set; }

        [Display(Name = "ContentID")]
        public string ContentID { get; set; }

        [Display(Name = "ContentType")]
        public string ContentType { get; set; }

        [Display(Name = "ContentCreated")]
        public string ContentCreated { get; set; }
    }
}
