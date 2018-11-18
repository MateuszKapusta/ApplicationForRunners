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
    public class MapPointItemController : TableController<MapPointItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            ApplicationForRunnersContext context = new ApplicationForRunnersContext();
            DomainManager = new EntityDomainManager<MapPointItem>(context, Request, enableSoftDelete: true);
        }

        // GET tables/MapPointItem
        public IQueryable<MapPointItem> GetAllMapPointItem()
        {
            var userId = new ValuesController().GetUserId();
            return Query().Where(de => de.UserId == userId);
        }

        // GET tables/MapPointItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MapPointItem> GetMapPointItem(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MapPointItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MapPointItem> PatchMapPointItem(string id, Delta<MapPointItem> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/MapPointItem
        public async Task<IHttpActionResult> PostMapPointItem(MapPointItem item)
        {
            item.UserId = new ValuesController().GetUserId();
            MapPointItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/MapPointItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMapPointItem(string id)
        {
             return DeleteAsync(id);
        }
    }
}
