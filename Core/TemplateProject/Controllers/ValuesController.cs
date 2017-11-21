using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TemplateProject.Api.Models;
using TemplateProject.Common;

namespace TemplateProject.Api.Controllers
{
    [Route("values")]
    public class ValuesController : Controller
    {
        private readonly IUrlHelper _urlHelper;
        private readonly AppSettings _settings;

        public ValuesController(IOptions<AppSettings> settings, IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
            _settings = settings.Value;
        }
        // GET api/values
        [HttpGet("", Name = "GetValues")]
        [HttpHead]
        public IActionResult GetValues(SearchParameters searchParameters = null)
        {
            List<PocoDto> result = new List<PocoDto>
                {
                    new PocoDto {Title = "Value 16", Id = 1},
                    new PocoDto {Title = "Value 15", Id = 2},
                    new PocoDto {Title = "Value 14", Id = 3},
                    new PocoDto {Title = "Value 13", Id = 4},
                    new PocoDto {Title = "Value 12", Id = 5},
                    new PocoDto {Title = "Value 11", Id = 6},
                    new PocoDto {Title = "Real", Id = 20},
                    new PocoDto {Title = "Value 10", Id = 7},
                    new PocoDto {Title = "Value 9", Id = 8},
                    new PocoDto {Title = "Value 8", Id = 9},
                    new PocoDto {Title = "Value 7", Id = 10},
                    new PocoDto {Title = "Value 6", Id = 11},
                    new PocoDto {Title = "Value 5", Id = 12},
                    new PocoDto {Title = "Value 4", Id = 13},
                    new PocoDto {Title = "Value 3", Id = 14},
                    new PocoDto {Title = "Value 2", Id = 15},
                    new PocoDto {Title = "Value 1", Id = 16},
                };

            if (searchParameters == null)
                searchParameters = new SearchParameters();

            if (!string.IsNullOrEmpty(searchParameters.OrderBy) && !Extensions.PropertyExists<PocoDto>(searchParameters.OrderBy))
            {
                return BadRequest();
            }
            else if (!string.IsNullOrEmpty(searchParameters.OrderByDesc) && !Extensions.PropertyExists<PocoDto>(searchParameters.OrderByDesc))
            {
                return BadRequest();
            }

            if (!string.IsNullOrEmpty(searchParameters.SearchParameter) && !Extensions.PropertyExists<PocoDto>(searchParameters.SearchParameter))
            {
                return BadRequest();
            }

            PagedList<PocoDto> pagedList = result.AsQueryable().CreatePagination(searchParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.CreateResourceUri(searchParameters, _urlHelper, nameof(GetValues))));

            return Ok(pagedList?.Select(PrepareResponse));
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetValue")]
        [HttpHead]
        [AllowAnonymous]
        public IActionResult GetValue(int id)
        {
            if (id == 0)
                return NotFound();

            return Ok(PrepareResponse(new PocoDto { Title = $"Value {id}", Id = id }));
        }

        // POST api/values
        [HttpPost("", Name = "CreateValue")]
        public IActionResult CreateValue([FromBody]string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("Invalid");

            return Ok(_settings.DefaultValue);
        }

        // PUT api/values/5
        [HttpPut("{id}", Name = "UpdateValue")]
        public IActionResult UpdateValue(int id, [FromBody]string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest();

            if (id == 0)
                return NotFound();

            return Accepted();
        }

        // DELETE api/values/5
        [HttpDelete("{id}", Name = "DeleteValue")]
        public IActionResult DeleteValue(int id)
        {
            return Accepted();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateForPoco")]
        public IActionResult PartiallyUpdateForPoco(int id, [FromBody] JsonPatchDocument<PocoDto> patchDoc)
        {
            var bookToPatch = new PocoDto();

            patchDoc.ApplyTo(bookToPatch, ModelState);

            if (bookToPatch.Description == bookToPatch.Title)
            {
                ModelState.AddModelError(nameof(PocoDto),
                    "The provided description should be different from the title.");
            }

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new StatusCodeResult(422);
            }


            return NoContent();
        }

        [HttpGet("{valueId}/products", Name = "GetValuesProducts")]
        public IActionResult GetValuesProducts(int valueId)
        {
            var result = new PocoDto
            {
                Title = "Value 1",
                Id = 1,
                Products = new List<PocoDto>
            {
               new PocoDto{Id = 132, Title = "Product 123"},
               new PocoDto{Id = 133, Title = "Product 123"},
            }
            };
            return Ok(PrepareResponse(result).Products);
        }

        [HttpGet("{valueId}/products/{productId}", Name = "GetValuesProduct")]
        public IActionResult GetValuesProduct(int valueId, int productId, PocoDto product)
        {
            return Ok(PrepareResponse(new PocoDto { Id = productId, Title = $"Product {productId}" }));
        }

        [HttpPatch("{valueId}/products/{productId}", Name = "PathProductForValue")]
        public IActionResult PathProductForValue(int valueId, int productId, [FromBody] JsonPatchDocument<PocoDto> patchDoc)
        {
            return Ok();
        }

        [HttpPut("{valueId}/products/{productId}", Name = "UpdateProductForValue")]
        public IActionResult UpdateProductForValue(int valueId, int productId, PocoDto product)
        {
            return Ok();
        }

        [HttpPost("{valueId}/products", Name = "CreateProductForValue")]
        public IActionResult CreateProductForValue(int valueId, PocoDto product)
        {
            return Ok();
        }

        private PocoDto PrepareResponse(PocoDto pocoDto)
        {
            List<LinkDto> links = new List<LinkDto>();
            links.Add(new LinkDto(_urlHelper.Link("GetValue", new { id = pocoDto.Id }), "self", "GET"));
            links.Add(new LinkDto(_urlHelper.Link("DeleteValue", new { id = pocoDto.Id }), "delete_value", "DELETE"));
            links.Add(new LinkDto(_urlHelper.Link("GetValuesProducts", new { valueId = pocoDto.Id }), "get_value_products", "GET"));
            links.Add(new LinkDto(_urlHelper.Link("CreateProductForValue", new { valueId = pocoDto.Id }), "create_product_value", "POST"));
            links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateForPoco", new { id = pocoDto.Id }), "patch_value", "PATCH"));
            pocoDto.Links = links;

            foreach (PocoDto pocoDtoProduct in pocoDto.Products)
            {
                List<LinkDto> producLinks = new List<LinkDto>();

                producLinks.Add(new LinkDto(_urlHelper.Link("GetValuesProduct", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "get_value_product", "GET"));
                producLinks.Add(new LinkDto(_urlHelper.Link("UpdateProductForValue", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "update_value_product", "PUT"));
                producLinks.Add(new LinkDto(_urlHelper.Link("PathProductForValue", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "patch_product_value", "PATCH"));
                pocoDtoProduct.Links = producLinks;
            }
            return pocoDto;
        }
    }
}

