﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using XCLCMS.Data.WebAPIEntity;
using XCLNetTools.Generic;

namespace XCLCMS.WebAPI.Controllers
{
    /// <summary>
    /// 角色管理
    /// </summary>
    public class SysRoleController : BaseAPIController
    {
        private XCLCMS.Data.BLL.SysRole sysRoleBLL = new Data.BLL.SysRole();
        private XCLCMS.Data.BLL.View.v_SysRole vSysRoleBLL = new Data.BLL.View.v_SysRole();
        private XCLCMS.Data.BLL.Merchant merchantBLL = new XCLCMS.Data.BLL.Merchant();

        /// <summary>
        /// 查询角色信息实体
        /// </summary>
        [HttpGet]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleView)]
        public APIResponseEntity<XCLCMS.Data.Model.SysRole> Detail([FromUri] string json)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<APIRequestEntity<long>>(System.Web.HttpUtility.UrlDecode(json));
            var response = new APIResponseEntity<XCLCMS.Data.Model.SysRole>();
            response.Body = this.sysRoleBLL.GetModel(request.Body);
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// 判断角色标识是否已经存在
        /// </summary>
        [HttpGet]
        public APIResponseEntity<bool> IsExistCode([FromUri] string json)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.SysRole.IsExistCodeEntity>>(System.Web.HttpUtility.UrlDecode(json));
            var response = new APIResponseEntity<bool>();
            response.IsSuccess = true;
            response.Message = "该标识可以使用！";

            XCLCMS.Data.Model.SysRole model = null;
            if (request.Body.SysRoleID > 0)
            {
                model = this.sysRoleBLL.GetModel(request.Body.SysRoleID);
                if (null != model)
                {
                    if (string.Equals(request.Body.Code, model.Code, StringComparison.OrdinalIgnoreCase))
                    {
                        return response;
                    }
                }
            }
            if (!string.IsNullOrEmpty(request.Body.Code))
            {
                bool isExist = this.sysRoleBLL.IsExistCode(request.Body.Code);
                if (isExist)
                {
                    response.IsSuccess = false;
                    response.Message = "该标识名已存在！";
                }
            }
            return response;
        }

        /// <summary>
        /// 判断角色名，在同一级别中是否存在
        /// </summary>
        [HttpGet]
        public APIResponseEntity<bool> IsExistRoleNameInSameLevel([FromUri] string json)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.SysRole.IsExistRoleNameInSameLevelEntity>>(System.Web.HttpUtility.UrlDecode(json));
            var response = new APIResponseEntity<bool>();
            response.IsSuccess = true;
            response.Message = "该角色名可以使用！";

            if (request.Body.SysRoleID > 0)
            {
                var model = this.sysRoleBLL.GetModel(request.Body.SysRoleID);
                if (null != model)
                {
                    if (string.Equals(request.Body.RoleName, model.RoleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return response;
                    }
                }
            }

            List<XCLCMS.Data.Model.SysRole> lst = this.sysRoleBLL.GetChildListByID(request.Body.ParentID);
            if (lst.IsNotNullOrEmpty())
            {
                if (lst.Exists(k => string.Equals(k.RoleName, request.Body.RoleName, StringComparison.OrdinalIgnoreCase)))
                {
                    response.IsSuccess = false;
                    response.Message = "该角色名在当前层级中已存在！";
                }
            }
            return response;
        }

        /// <summary>
        /// 查询所有角色列表
        /// </summary>
        [HttpGet]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleView)]
        public APIResponseEntity<List<XCLCMS.Data.Model.View.v_SysRole>> GetList(string json)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<APIRequestEntity<long>>(System.Web.HttpUtility.UrlDecode(json));
            var response = new APIResponseEntity<List<XCLCMS.Data.Model.View.v_SysRole>>();
            response.Body = this.vSysRoleBLL.GetList(request.Body);
            response.IsSuccess = true;
            return response;
        }

        /// <summary>
        /// 获取easyui tree格式的所有角色json
        /// </summary>
        public APIResponseEntity<List<XCLNetTools.Entity.EasyUI.TreeItem>> GetAllJsonForEasyUITree([FromUri] string json)
        {
            var request = Newtonsoft.Json.JsonConvert.DeserializeObject<APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.SysRole.GetAllJsonForEasyUITreeEntity>>(System.Web.HttpUtility.UrlDecode(json));
            var response = new APIResponseEntity<List<XCLNetTools.Entity.EasyUI.TreeItem>>();
            response.IsSuccess = true;

            List<XCLCMS.Data.Model.View.v_SysRole> allData = null;
            List<XCLNetTools.Entity.EasyUI.TreeItem> tree = new List<XCLNetTools.Entity.EasyUI.TreeItem>();

            var merchantModel = this.merchantBLL.GetModel(request.Body.MerchantID);
            if (null == merchantModel)
            {
                response.IsSuccess = false;
                response.Message = "您指定的商户号无效！";
                return response;
            }

            allData = this.vSysRoleBLL.GetModelList("");

            if (allData.IsNotNullOrEmpty())
            {
                var root = allData.Where(k => k.ParentID == 0).FirstOrDefault();//根节点
                if (null != root)
                {
                    tree.Add(new XCLNetTools.Entity.EasyUI.TreeItem()
                    {
                        ID = root.SysRoleID.ToString(),
                        State = root.IsLeaf == 1 ? "open" : "closed",
                        Text = root.RoleName
                    });

                    Action<XCLNetTools.Entity.EasyUI.TreeItem> getChildAction = null;
                    getChildAction = new Action<XCLNetTools.Entity.EasyUI.TreeItem>((parentModel) =>
                    {
                        var childs = allData.Where(k => k.ParentID == Convert.ToInt64(parentModel.ID)).ToList();
                        if (childs.IsNotNullOrEmpty())
                        {
                            childs = childs.OrderBy(k => k.Weight).ToList();
                            parentModel.Children = new List<XCLNetTools.Entity.EasyUI.TreeItem>();
                            childs.ForEach(m =>
                            {
                                var treeItem = new XCLNetTools.Entity.EasyUI.TreeItem()
                                {
                                    ID = m.SysRoleID.ToString(),
                                    State = m.IsLeaf == 1 ? "open" : "closed",
                                    Text = m.RoleName
                                };
                                getChildAction(treeItem);
                                parentModel.Children.Add(treeItem);
                            });
                        }
                    });

                    //从根节点开始
                    getChildAction(tree[0]);
                }
            }
            response.Body = tree;
            return response;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        [HttpPost]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleAdd)]
        public APIResponseEntity<bool> Add(JObject obj)
        {
            var request = obj.ToObject<APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.SysRole.AddOrUpdateEntity>>();
            var response = new APIResponseEntity<bool>();
            var allLeafFunctionIds = XCLCMS.Lib.Permission.PerHelper.GetFunctionList().Where(k => k.IsLeaf == 1).Select(k => (long)k.SysFunctionID).ToList();

            #region 数据校验

            if (null == request.Body.SysRole)
            {
                response.IsSuccess = false;
                response.Message = "请指定角色信息！";
                return response;
            }
            request.Body.SysRole.RoleName = (request.Body.SysRole.RoleName ?? "").Trim();
            request.Body.SysRole.Code = (request.Body.SysRole.Code ?? "").Trim();
            if (null == request.Body.FunctionIdList)
            {
                request.Body.FunctionIdList = new List<long>();
            }

            //商户必须存在
            var merchant = this.merchantBLL.GetModel(request.Body.SysRole.FK_MerchantID);
            if (null == merchant)
            {
                response.IsSuccess = false;
                response.Message = "无效的商户号！";
                return response;
            }

            //追加默认的功能权限
            if (string.Equals(request.Body.SysRole.Code, XCLCMS.Data.CommonHelper.SysRoleConst.SysRoleCodeEnum.MerchantMainRole.ToString(), StringComparison.OrdinalIgnoreCase) || merchant.MerchantSystemType == XCLCMS.Data.CommonHelper.EnumType.MerchantSystemTypeEnum.NOR.ToString())
            {
                request.Body.FunctionIdList.AddRange(XCLCMS.Lib.Permission.PerHelper.NormalMerchantFixedFunctionIDList);
            }
            request.Body.FunctionIdList = request.Body.FunctionIdList.Intersect(allLeafFunctionIds).Distinct().ToList();

            //必须指定角色信息
            if (string.IsNullOrEmpty(request.Body.SysRole.RoleName))
            {
                response.IsSuccess = false;
                response.Message = "请指定角色名！";
                return response;
            }

            //角色code是否存在
            if (!string.IsNullOrEmpty(request.Body.SysRole.Code))
            {
                if (this.sysRoleBLL.IsExistCode(request.Body.SysRole.Code))
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("角色标识【{0}】已存在！", request.Body.SysRole.Code);
                    return response;
                }
            }

            //父角色是否存在
            var parentNodeModel = this.sysRoleBLL.GetModel(request.Body.SysRole.ParentID);
            if (null == parentNodeModel)
            {
                response.IsSuccess = false;
                response.Message = "父角色不存在！";
                return response;
            }

            //当前用户只能加在自己的商户号下面
            if (this.vSysRoleBLL.IsRoot(parentNodeModel.SysRoleID))
            {
                if (request.Body.SysRole.FK_MerchantID != base.CurrentUserModel.FK_MerchantID)
                {
                    response.IsSuccess = false;
                    response.Message = "只能添加自己的商户角色！";
                    return response;
                }
            }
            else
            {
                if (parentNodeModel.FK_MerchantID != request.Body.SysRole.FK_MerchantID)
                {
                    response.IsSuccess = false;
                    response.Message = "您添加的角色必须与父角色在同一个商户中！";
                    return response;
                }
            }

            //普通商户的权限是否已越界
            if (merchant.MerchantSystemType == XCLCMS.Data.CommonHelper.EnumType.MerchantSystemTypeEnum.NOR.ToString())
            {
                var normalFunIds = XCLCMS.Lib.Permission.PerHelper.GetNormalMerchantFunctionIDList();
                if (request.Body.FunctionIdList.IsNotNullOrEmpty())
                {
                    if (request.Body.FunctionIdList.Exists(k => !normalFunIds.Contains(k)))
                    {
                        response.IsSuccess = false;
                        response.Message = "该角色的权限已越界！";
                        return response;
                    }
                }
            }

            #endregion 数据校验

            XCLCMS.Data.BLL.Strategy.SysRole.SysRoleContext sysRoleContext = new Data.BLL.Strategy.SysRole.SysRoleContext();
            sysRoleContext.CurrentUserInfo = base.CurrentUserModel;
            sysRoleContext.SysRole = request.Body.SysRole;
            sysRoleContext.FunctionIdList = request.Body.FunctionIdList;
            sysRoleContext.HandleType = Data.BLL.Strategy.StrategyLib.HandleType.ADD;

            XCLCMS.Data.BLL.Strategy.ExecuteStrategy strategy = new Data.BLL.Strategy.ExecuteStrategy(new List<Data.BLL.Strategy.BaseStrategy>() {
                new XCLCMS.Data.BLL.Strategy.SysRole.SysRole(),
                new XCLCMS.Data.BLL.Strategy.SysRole.SysRoleFunction()
            });
            strategy.Execute<XCLCMS.Data.BLL.Strategy.SysRole.SysRoleContext>(sysRoleContext);

            if (strategy.Result != Data.BLL.Strategy.StrategyLib.ResultEnum.FAIL)
            {
                response.Message = "添加成功！";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = strategy.ResultMessage;
                response.IsSuccess = false;
                XCLNetLogger.Log.WriteLog(XCLNetLogger.Config.LogConfig.LogLevel.ERROR, "添加角色信息失败", strategy.ResultMessage);
            }

            return response;
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [HttpPost]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleEdit)]
        public APIResponseEntity<bool> Update(JObject obj)
        {
            var request = obj.ToObject<APIRequestEntity<XCLCMS.Data.WebAPIEntity.RequestEntity.SysRole.AddOrUpdateEntity>>();
            var response = new APIResponseEntity<bool>();
            var allLeafFunctionIds = XCLCMS.Lib.Permission.PerHelper.GetFunctionList().Where(k => k.IsLeaf == 1).Select(k => (long)k.SysFunctionID).ToList();

            #region 数据校验

            if (null == request.Body.SysRole)
            {
                response.IsSuccess = false;
                response.Message = "请指定角色信息！";
                return response;
            }

            var model = this.sysRoleBLL.GetModel(request.Body.SysRole.SysRoleID);
            if (null == model)
            {
                response.IsSuccess = false;
                response.Message = "请指定有效的角色信息！";
                return response;
            }

            request.Body.SysRole.RoleName = (request.Body.SysRole.RoleName ?? "").Trim();
            request.Body.SysRole.Code = (request.Body.SysRole.Code ?? "").Trim();
            if (null == request.Body.FunctionIdList)
            {
                request.Body.FunctionIdList = new List<long>();
            }

            //商户必须存在
            var merchant = this.merchantBLL.GetModel(request.Body.SysRole.FK_MerchantID);
            if (null == merchant)
            {
                response.IsSuccess = false;
                response.Message = "无效的商户号！";
                return response;
            }

            //追加默认的功能权限
            if (string.Equals(request.Body.SysRole.Code, XCLCMS.Data.CommonHelper.SysRoleConst.SysRoleCodeEnum.MerchantMainRole.ToString(), StringComparison.OrdinalIgnoreCase) || merchant.MerchantSystemType == XCLCMS.Data.CommonHelper.EnumType.MerchantSystemTypeEnum.NOR.ToString())
            {
                request.Body.FunctionIdList.AddRange(XCLCMS.Lib.Permission.PerHelper.NormalMerchantFixedFunctionIDList);
            }
            request.Body.FunctionIdList = request.Body.FunctionIdList.Intersect(allLeafFunctionIds).ToList();

            //必须指定角色信息
            if (string.IsNullOrEmpty(request.Body.SysRole.RoleName))
            {
                response.IsSuccess = false;
                response.Message = "请指定角色名！";
                return response;
            }

            //角色code是否存在
            if (!string.IsNullOrEmpty(request.Body.SysRole.Code))
            {
                if (!string.Equals(model.Code, request.Body.SysRole.Code, StringComparison.OrdinalIgnoreCase) && this.sysRoleBLL.IsExistCode(request.Body.SysRole.Code))
                {
                    response.IsSuccess = false;
                    response.Message = string.Format("角色标识【{0}】已存在！", request.Body.SysRole.Code);
                    return response;
                }
            }

            //普通商户的权限是否已越界
            if (merchant.MerchantSystemType == XCLCMS.Data.CommonHelper.EnumType.MerchantSystemTypeEnum.NOR.ToString())
            {
                var normalFunIds = XCLCMS.Lib.Permission.PerHelper.GetNormalMerchantFunctionIDList();
                if (request.Body.FunctionIdList.IsNotNullOrEmpty())
                {
                    if (request.Body.FunctionIdList.Exists(k => !normalFunIds.Contains(k)))
                    {
                        response.IsSuccess = false;
                        response.Message = "该角色的权限已越界！";
                        return response;
                    }
                }
            }

            #endregion 数据校验

            model.Code = request.Body.SysRole.Code;
            model.RecordState = request.Body.SysRole.RecordState;
            model.Remark = request.Body.SysRole.Remark;
            model.RoleName = request.Body.SysRole.RoleName;
            model.Sort = request.Body.SysRole.Sort;
            model.UpdaterID = base.CurrentUserModel.UserInfoID;
            model.UpdaterName = base.CurrentUserModel.UserName;
            model.UpdateTime = DateTime.Now;
            model.Weight = request.Body.SysRole.Weight;

            XCLCMS.Data.BLL.Strategy.SysRole.SysRoleContext sysRoleContext = new Data.BLL.Strategy.SysRole.SysRoleContext();
            sysRoleContext.CurrentUserInfo = base.CurrentUserModel;
            sysRoleContext.SysRole = model;
            sysRoleContext.FunctionIdList = request.Body.FunctionIdList;
            sysRoleContext.HandleType = Data.BLL.Strategy.StrategyLib.HandleType.UPDATE;

            XCLCMS.Data.BLL.Strategy.ExecuteStrategy strategy = new Data.BLL.Strategy.ExecuteStrategy(new List<Data.BLL.Strategy.BaseStrategy>() {
                new XCLCMS.Data.BLL.Strategy.SysRole.SysRole(),
                new XCLCMS.Data.BLL.Strategy.SysRole.SysRoleFunction()
            });
            strategy.Execute<XCLCMS.Data.BLL.Strategy.SysRole.SysRoleContext>(sysRoleContext);

            if (strategy.Result != Data.BLL.Strategy.StrategyLib.ResultEnum.FAIL)
            {
                response.Message = "修改成功！";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = strategy.ResultMessage;
                response.IsSuccess = false;
                XCLNetLogger.Log.WriteLog(XCLNetLogger.Config.LogConfig.LogLevel.ERROR, "修改角色信息失败", strategy.ResultMessage);
            }

            return response;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpPost]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleDel)]
        public APIResponseEntity<bool> Delete(JObject obj)
        {
            var request = obj.ToObject<APIRequestEntity<List<long>>>();
            var response = new APIResponseEntity<bool>();

            if (null == request.Body || request.Body.Count == 0)
            {
                response.IsSuccess = false;
                response.Message = "请指定要删除的角色ID！";
                return response;
            }

            request.Body = request.Body.Distinct().ToList();

            //只能删除自己商户的节点
            if (request.Body.Exists(id =>
            {
                var sysRoleModel = this.sysRoleBLL.GetModel(id);
                return null != sysRoleModel && sysRoleModel.FK_MerchantID != base.CurrentUserModel.FK_MerchantID;
            }))
            {
                response.IsSuccess = false;
                response.Message = "只能删除属于自己的商户节点！";
                return response;
            }

            int successCount = 0;

            request.Body.ForEach(id =>
            {
                var sysRoleModel = this.sysRoleBLL.GetModel(id);
                if (null != sysRoleModel)
                {
                    sysRoleModel.UpdaterID = base.CurrentUserModel.UserInfoID;
                    sysRoleModel.UpdaterName = base.CurrentUserModel.UserName;
                    sysRoleModel.UpdateTime = DateTime.Now;
                    sysRoleModel.RecordState = XCLCMS.Data.CommonHelper.EnumType.RecordStateEnum.D.ToString();
                    if (this.sysRoleBLL.Update(sysRoleModel))
                    {
                        successCount++;
                    }
                }
            });

            response.IsSuccess = true;
            response.Message = string.Format("已成功删除【{0}】条记录！", successCount);

            return response;
        }

        /// <summary>
        /// 删除指定角色的所有节点
        /// </summary>
        [HttpPost]
        [XCLCMS.Lib.Filters.FunctionFilter(Function = XCLCMS.Lib.Permission.Function.FunctionEnum.SysFun_SysRoleDel)]
        public APIResponseEntity<bool> DelChild(JObject obj)
        {
            var request = obj.ToObject<APIRequestEntity<long>>();
            var response = new APIResponseEntity<bool>();

            if (request.Body <= 0)
            {
                response.IsSuccess = false;
                response.Message = "请指定要删除所有子节点的角色ID！";
                return response;
            }

            var sysRoleModel = sysRoleBLL.GetModel(request.Body);
            if (null != sysRoleModel && sysRoleModel.FK_MerchantID != base.CurrentUserModel.FK_MerchantID)
            {
                response.IsSuccess = false;
                response.Message = "只能删除属于自己的商户节点！";
                return response;
            }

            response.IsSuccess = this.sysRoleBLL.DelChild(new Data.Model.SysRole()
            {
                SysRoleID = request.Body,
                UpdaterID = base.CurrentUserModel.UserInfoID,
                UpdaterName = base.CurrentUserModel.UserName,
                UpdateTime = DateTime.Now
            });

            if (response.IsSuccess)
            {
                response.Message = "成功删除所有子节点！";
            }
            else
            {
                response.Message = "删除所有子节点失败！";
            }

            return response;
        }
    }
}