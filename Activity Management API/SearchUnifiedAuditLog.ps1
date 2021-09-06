Install-Module -Name ExchangeOnlineManagement
Connect-ExchangeOnline
Search-UnifiedAuditLog -StartDate 8/4/2021 -EndDate 9/4/2021 -RecordType Sharepoint

Search-UnifiedAuditLog -StartDate 8/4/2021 -EndDate 9/4/2021 -RecordType Sharepoint | Measure
Search-UnifiedAuditLog -StartDate 8/4/2021 -EndDate 9/4/2021 -RecordType Sharepoint | Where { ($_.Operations -eq "PageViewed")} | Measure
Search-UnifiedAuditLog -StartDate 8/4/2021 -EndDate 9/4/2021 -RecordType Sharepoint | Where { ($_.Operations -eq "PageViewed") -and ($_.UserIds -eq "henry.jones@iberiahero.com") } | Measure