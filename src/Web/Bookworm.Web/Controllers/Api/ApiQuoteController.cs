﻿namespace Bookworm.Web.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using Bookworm.Data.Models;
    using Bookworm.Services.Data.Contracts.Quotes;
    using Bookworm.Services.Mapping;
    using Bookworm.Web.ViewModels.DTOs;
    using Bookworm.Web.ViewModels.Quotes;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class ApiQuoteController : ApiBaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRetrieveQuotesService retrieveQuotesService;
        private readonly IManageQuoteLikesService manageQuoteLikesService;

        public ApiQuoteController(
            UserManager<ApplicationUser> userManager,
            IRetrieveQuotesService retrieveQuotesService,
            IManageQuoteLikesService manageQuoteLikesService)
        {
            this.userManager = userManager;
            this.retrieveQuotesService = retrieveQuotesService;
            this.manageQuoteLikesService = manageQuoteLikesService;
        }

        [HttpGet(nameof(GetQuotes))]
        public async Task<IActionResult> GetQuotes([FromQuery]GetQuotesApiRequestModel model)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                var quotesApiDto = AutoMapperConfig.MapperInstance.Map<GetQuotesApiDto>(model);
                var quotesModel = await this.retrieveQuotesService.GetAllByCriteriaAsync(userId, quotesApiDto);

                return new JsonResult(quotesModel) { StatusCode = 200 };
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpPost(nameof(LikeQuote))]
        public async Task<ActionResult<int>> LikeQuote(int quoteId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                return await this.manageQuoteLikesService.LikeAsync(quoteId, userId);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpDelete(nameof(UnlikeQuote))]
        public async Task<ActionResult<int>> UnlikeQuote(int quoteId)
        {
            try
            {
                string userId = this.userManager.GetUserId(this.User);
                return await this.manageQuoteLikesService.UnlikeAsync(quoteId, userId);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }
    }
}
