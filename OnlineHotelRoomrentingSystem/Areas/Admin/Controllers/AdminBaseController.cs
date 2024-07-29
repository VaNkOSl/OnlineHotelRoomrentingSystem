namespace OnlineHotelRoomrentingSystem.Areas.Admin.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static OnlineHotelRoomrentingSystem.Commons.GeneralApplicationConstants;

[Area(AdminAreaName)]
[Authorize(Roles = AdminRoleName)]
public class AdminBaseController : Controller
{

}
