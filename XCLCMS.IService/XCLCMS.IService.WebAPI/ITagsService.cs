﻿using System.Collections.Generic;
using XCLCMS.Data.Model.Custom;
using XCLCMS.Data.Model.View;
using XCLCMS.Data.WebAPIEntity;
using XCLCMS.Data.WebAPIEntity.RequestEntity;
using XCLCMS.Data.WebAPIEntity.RequestEntity.Tags;
using XCLCMS.Data.WebAPIEntity.ResponseEntity;

namespace XCLCMS.IService.WebAPI
{
    /// <summary>
    /// 标签服务
    /// </summary>
    public interface ITagsService : IBaseInfoService
    {
        APIResponseEntity<bool> Add(APIRequestEntity<Data.Model.Tags> request);

        APIResponseEntity<bool> Delete(APIRequestEntity<List<long>> request);

        APIResponseEntity<Data.Model.Tags> Detail(APIRequestEntity<long> request);

        APIResponseEntity<bool> IsExistTagName(APIRequestEntity<IsExistTagNameEntity> request);

        APIResponseEntity<PageListResponseEntity<v_Tags>> PageList(APIRequestEntity<PageListConditionEntity> request);

        APIResponseEntity<bool> Update(APIRequestEntity<Data.Model.Tags> request);

        APIResponseEntity<List<Data.Model.Tags>> GetObjectTags(APIRequestEntity<Tags_ObjectTagsCondition> request);
    }
}