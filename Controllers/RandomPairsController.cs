using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RandomPairsApp.Models;
using System.Linq;
using RandomPairsApp.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RandomPairsApp.Controllers
{
    [Route("api/[controller]")]
    public class RandomPairsController : Controller
    {
        private RandomPairsServiceProvider _randomPairsServiceProvider;

        public RandomPairsController(RandomPairsServiceProvider randomPairsServiceProvider = null)
        {
            _randomPairsServiceProvider = randomPairsServiceProvider;
        }

        // Get the latest 20 pairs
        [HttpGet("get-latest")]
        public IActionResult GetLatestPairs()
        {
            try
            {
                var listSnapshot = _randomPairsServiceProvider.GetPairsListSnapshot();
                int length = listSnapshot.Count;
                int startListingIndex = (int)MathF.Max(0, length - 20);
                int count = (int)MathF.Min(20, length);

                var subList = _randomPairsServiceProvider.RandomPairsList.GetRange(startListingIndex, count);

                return Ok(subList.Select(x => new[] { x.Second, x.Value }).ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("get-sum")]
        public async Task<IActionResult> GetSum()
        {
            try
            {
                SumResult sumResult = null;
                Task t = new Task(() =>
                {
                    sumResult = _randomPairsServiceProvider.GetSum();
                });

                t.Start();
                await t;

                if (sumResult != null)
                    return Ok(sumResult);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("get-median")]
        public IActionResult GetMedian()
        {
            try
            {
                float median = _randomPairsServiceProvider.GetMedian();
                if (median == 0)
                {
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent);
                }
                else
                    return Ok(median);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("get-sum-count")]
        public IActionResult GetSumCount()
        {
            try
            {
                int? sumCount = _randomPairsServiceProvider.GetSumCount();
                if (sumCount.HasValue)
                    return Ok(sumCount);
                else
                    return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError);
            }
        }
    }
}