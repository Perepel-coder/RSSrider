using Microsoft.AspNetCore.Mvc;

namespace RSSrider.RSS.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ClassMainController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> RssUrlsControl()
        {
            return Ok(await ClassRSSLoad.RssUrls());
        }

        [HttpGet]
        public async Task<ActionResult> RSSTimerControl()
        {
            return Ok(await ClassRSSLoad.RSSTimer());
        }

        [HttpPost]
        public async Task<ActionResult> AddFeedControl(string[] url)
        {
            ClassRSSLoad.AddFeed(url[0]);
            return Ok(await ClassRSSLoad.GetFeed(url[0]));
        }

        [HttpPost]
        public async Task<ActionResult> DelFeedControl(string[] urls)
        {
            return Ok(await ClassRSSLoad.DelFeed(urls));
        }

        [HttpPost]
        public async Task<ActionResult> ChangeStatusControl(string[] urls)
        {
            return Ok(await ClassRSSLoad.ChangeStatus(urls));
        }
    }
}
