using System.ComponentModel.DataAnnotations;

namespace O365.ActivityManagementAPI.Entities
{
    public class SharePointDataAudit
    {
        [Display(Name = "CreationTime")]
        public string CreationTime { get; set; }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Operation")]
        public string Operation { get; set; }

        [Display(Name = "OrganizationId")]
        public string OrganizationId { get; set; }

        [Display(Name = "RecordType")]
        public string RecordType { get; set; }

        [Display(Name = "UserKey")]
        public string UserKey { get; set; }

        [Display(Name = "UserType")]
        public string UserType { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }

        [Display(Name = "Workload")]
        public string Workload { get; set; }

        [Display(Name = "ClientIP")]
        public string ClientIp { get; set; }

        [Display(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Display(Name = "CorrelationId")]
        public string CorrelationId { get; set; }

        [Display(Name = "EventSource")]
        public string EventSource { get; set; }

        [Display(Name = "ItemType")]
        public string ItemType { get; set; }

        [Display(Name = "ListID")]
        public string ListID { get; set; }

        [Display(Name = "ListItemUniqueId")]
        public string ListItemUniqueId { get; set; }

        [Display(Name = "Site")]
        public string Site { get; set; }

        [Display(Name = "UserAgent")]
        public string UserAgent { get; set; }

        [Display(Name = "WebId")]
        public string WebID { get; set; }

        [Display(Name = "SourceFileExtension")]
        public string SourceFileExtension { get; set; }

        [Display(Name = "SiteUrl")]
        public string SiteUrl { get; set; }

        [Display(Name = "SourceFileName")]
        public string SourceFileName { get; set; }

        [Display(Name = "SourceRelativeURL")]
        public string SourceRelativeURL { get; set; }

        [Display(Name = "Department")]
        public string Department { get; set; }

        [Display(Name = "PreferredLanguage")]
        public string PreferredLanguage { get; set; }

        [Display(Name = "OfficeLocation")]
        public string OfficeLocation { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "JobTitle")]
        public string JobTitle { get; set; }
    }
}
