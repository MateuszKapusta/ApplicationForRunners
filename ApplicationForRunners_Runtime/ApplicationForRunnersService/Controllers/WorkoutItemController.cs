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
    public class WorkoutItemController : TableController<WorkoutItem>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            ApplicationForRunnersContext context = new ApplicationForRunnersContext();
            DomainManager = new EntityDomainManager<WorkoutItem>(context, Request, enableSoftDelete: true);
        }

        [HttpGet]
        // GET tables/WorkoutItem
        public IQueryable<WorkoutItem> GetAllWorkoutItems()
        {
            var userId = new ValuesController().GetUserId();
            return Query().Where(de => de.UserId == userId);
        }
        [HttpGet]
        // GET tables/WorkoutItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<WorkoutItem> GetWorkoutItem(string id)
        {
            return Lookup(id);
        }
        [HttpPatch]
        // PATCH tables/WorkoutItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<WorkoutItem> PatchWorkoutItem(string id, Delta<WorkoutItem> patch)
        {
            return UpdateAsync(id, patch);
        }
        [HttpPost]
        // POST tables/WorkoutItem
        public async Task<IHttpActionResult> PostWorkoutItem(WorkoutItem item)
        {
            item.UserId = new ValuesController().GetUserId();
            WorkoutItem current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }
        [HttpDelete]
        // DELETE tables/WorkoutItem/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteWorkoutItem(string id)
        {
            return DeleteAsync(id);
        }
    }
}