### Use Case
> Simplifying the connection and communication to machine with common protocol.
### Environment
> Dotnet 8 Blazor Server-Side with Devexpress style
> SQL server
### DB preparatory works
> Using machineDB.sql file to create necessary table schema.
> Check your SQL server can login with sql authentication.
### Code setting
appsettings.json
```
{
  "ConnectionStrings":
    {
      "connection string name": "connection string value" //Data Source=ip;Initial Catalog=db name;User ID=sa;Password=**********;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False
    },
  "othersetting"
  {
    ...
  },
  ...
}
```
Progeam.cs
```
builder.AddMachineService(); //if your connection string name is "DefaultConnection"
or
builder.AddMachineService(your connection string); //if your connection string name is not "DefaultConnection"
```
### Component setting
* Put following components in your page.
* Set machines and tags configuration in DB with following components.

```
<MachineSetting/>
<TagCategoriesSetting/>
```
### Verify setting
* Put following components in your page.
* Verify machine information is accessible with following component.
```
<MachineDashboard Machine="@machine object"/>
```
### Verify service
* Inject MachineService to your own service (or component) which is already injected with singleton lifecycle in previous steps.
how to get tag value
```
Tag? tag = await MachineService.GetMachineTag(string machineName, string tagName);
Object? val = tag?.Value;//need to downcast by yourself
or
string s = tag?.ValueString();
```
how to set tag value
```
RequestResult res = await MachineService.SetMachineTag(string machineName, string tagName, object val);
```
![ㄏㄏ](https://imgcdn.sigstick.com/mPcgGpNRlLbwjPCSCtxC/0-1.png)
