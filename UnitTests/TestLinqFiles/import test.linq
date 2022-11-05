<Query Kind="Program">
  <NuGetReference>ADOInfo.Lib</NuGetReference>
  <Namespace>ADOInfo.Lib.AdoModels</Namespace>
  <Namespace>ADOInfo.Lib.AdoModels.Definitions</Namespace>
  <Namespace>ADOInfo.Lib.AdoModels.Projects</Namespace>
  <Namespace>ADOInfo.Lib.Helpers</Namespace>
  <Namespace>ADOInfo.Lib.OutputModels</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

// <summary>some stuff</summary>
#load "ADO\lib.ExpandGroupMembership"

///
/// You will need access to the ADO Organization, but not any of the Projects.
/// This script will dump out all hidden projects and their members
/// <summary></summary>
#load  "ADO\Archive\Query Temp Kusto Table"
#load  "ADO\Build and Release Log Downloader\JWT_SearchInLogs"


Main()
{
    string adoOrg = "office";
}

