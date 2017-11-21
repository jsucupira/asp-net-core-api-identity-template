using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Routing;
using JsonPatch;
using Newtonsoft.Json;
using TemplateProject.Api.Models;
using TemplateProject.Common;

namespace TemplateProject.Api.Controllers
{
    [RoutePrefix("values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [Route("", Name = nameof(GetValues))]
        [HttpGet]
        [HttpHead]
        public IHttpActionResult GetValues([FromUri]SearchParameters searchParameters = null)
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

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, pagedList?.Select(PrepareResponse));
            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.CreateResourceUri(searchParameters, Request, nameof(GetValues))));

            return ResponseMessage(response);
        }

        // GET api/values/5
        [Route("{id}", Name = "GetValue")]
        [HttpGet]
        [HttpHead]
        public IHttpActionResult GetValue(int id)
        {
            if (id == 0)
                return NotFound();

            return Ok(PrepareResponse(new PocoDto { Title = $"Value {id}", Id = id }));
        }

        // POST api/values
        [Route("", Name = "CreateValue")]
        [HttpPost]
        public IHttpActionResult CreateValue([FromBody]string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("Invalid");

            return Ok(Settings.DefaultValue);
        }

        // PUT api/values/5
        [Route("{id}", Name = "UpdateValue")]
        [HttpPut]
        public IHttpActionResult UpdateValue(int id, [FromBody]string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest();

            if (id == 0)
                return NotFound();

            return StatusCode(HttpStatusCode.Accepted);
        }

        // DELETE api/values/5
        [Route("{id}", Name = "DeleteValue")]
        [HttpDelete]
        public IHttpActionResult DeleteValue(int id)
        {
            return StatusCode(HttpStatusCode.Accepted);
        }

        [Route("{id}", Name = "PartiallyUpdateForPoco")]
        [HttpPatch]
        public IHttpActionResult PartiallyUpdateForPoco(int id, [FromBody] JsonPatchDocument<PocoDto> patchDoc)
        {
            var bookToPatch = new PocoDto();

            patchDoc.ApplyUpdatesTo(bookToPatch);

            if (bookToPatch.Description == bookToPatch.Title)
            {
                ModelState.AddModelError(nameof(PocoDto),
                    "The provided description should be different from the title.");
            }

            Validate(bookToPatch);

            if (!ModelState.IsValid)
            {
                return StatusCode((HttpStatusCode)422);
            }
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{valueId}/products", Name = "GetValuesProducts")]
        [HttpGet]
        public IHttpActionResult GetValuesProducts(int valueId)
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

        [Route("{valueId}/products/{productId}", Name = "GetValuesProduct")]
        [HttpGet]
        public IHttpActionResult GetValuesProduct(int valueId, int productId, PocoDto product)
        {
            return Ok(PrepareResponse(new PocoDto { Id = productId, Title = $"Product {productId}" }));
        }

        [Route("{valueId}/products/{productId}", Name = "PathProductForValue")]
        [HttpPatch]
        public IHttpActionResult PathProductForValue(int valueId, int productId, [FromBody] JsonPatchDocument<PocoDto> patchDoc)
        {
            return Ok();
        }

        [Route("{valueId}/products/{productId}", Name = "UpdateProductForValue")]
        [HttpPut]
        public IHttpActionResult UpdateProductForValue(int valueId, int productId, PocoDto product)
        {
            return Ok();
        }

        [Route("{valueId}/products", Name = "CreateProductForValue")]
        [HttpPost]
        public IHttpActionResult CreateProductForValue(int valueId, PocoDto product)
        {
            return Ok();
        }

        private PocoDto PrepareResponse(PocoDto pocoDto)
        {
            UrlHelper urlHelper = new UrlHelper(Request);
            List<LinkDto> links = new List<LinkDto>();
            links.Add(new LinkDto(urlHelper.Link("GetValue", new { id = pocoDto.Id }), "self", "GET"));
            links.Add(new LinkDto(urlHelper.Link("DeleteValue", new { id = pocoDto.Id }), "delete_value", "DELETE"));
            links.Add(new LinkDto(urlHelper.Link("GetValuesProducts", new { valueId = pocoDto.Id }), "get_value_products", "GET"));
            links.Add(new LinkDto(urlHelper.Link("CreateProductForValue", new { valueId = pocoDto.Id }), "create_product_value", "POST"));
            links.Add(new LinkDto(urlHelper.Link("PartiallyUpdateForPoco", new { id = pocoDto.Id }), "patch_value", "PATCH"));
            pocoDto.Links = links;

            foreach (PocoDto pocoDtoProduct in pocoDto.Products)
            {
                List<LinkDto> producLinks = new List<LinkDto>();

                producLinks.Add(new LinkDto(urlHelper.Link("GetValuesProduct", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "get_value_product", "GET"));
                producLinks.Add(new LinkDto(urlHelper.Link("UpdateProductForValue", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "update_value_product", "PUT"));
                producLinks.Add(new LinkDto(urlHelper.Link("PathProductForValue", new { valueId = pocoDto.Id, productId = pocoDtoProduct.Id }), "patch_product_value", "PATCH"));
                pocoDtoProduct.Links = producLinks;
            }
            return pocoDto;
        }
    }
}

