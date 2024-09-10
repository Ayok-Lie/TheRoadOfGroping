using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoadOfGroping.Core.ZRoadOfGropingUtility.ResultResponse
{
    internal class FileActionResultWarp : IActionResultWarp
    {
        public void Wrap(FilterContext context)
        {
            FileResult result = null;

            switch (context)
            {
                case ResultExecutingContext resultExecutingContext:
                    result = resultExecutingContext.Result as FileStreamResult;
                    break;
            }

            if (result == null)
            {
                throw new ArgumentException("Action Result should be JsonResult!");
            }
        }
    }
}