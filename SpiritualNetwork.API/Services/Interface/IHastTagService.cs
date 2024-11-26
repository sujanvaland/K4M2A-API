﻿using SpiritualNetwork.Entities.CommonModel;

namespace SpiritualNetwork.API.Services.Interface
{
    public interface IHastTagService
    {
        public Task<JsonResponse> ExtractPostHashTag(int postId);
    }
}