using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using ApplicationForRunnersService.DataObjects;
using ApplicationForRunnersService.Models;


namespace ApplicationForRunnersService.Controllers
{
    [Authorize]
    public class UserItemController : TableController<UserItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            ApplicationForRunnersContext context = new ApplicationForRunnersContext();
            DomainManager = new EntityDomainManager<UserItem>(context, Request,enableSoftDelete: true);
        }
        [HttpGet]
        // GET tables/UserItem
        public IQueryable<UserItem> GetAllUserItems()
        {
            var userId = new ValuesController().GetUserId();
            return Query().Where(de => de.UserId == userId);
        }
        [HttpGet]
        // GET tables/UserItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<UserItem> GetUserItem(string id)
        {
            return Lookup(id);
        }
        [HttpPatch]
        // PATCH tables/UserItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<UserItem> PatchUserItem(string id, Delta<UserItem> patch)
        {
            return UpdateAsync(id, patch);
        }
        [HttpPost]
        // POST tables/UserItem
        public async Task<IHttpActionResult> PostUserItem(UserItem item)
        {
            item.UserId = new ValuesController().GetUserId();
            UserItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        [HttpDelete]
        // DELETE tables/UserItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteUserItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}