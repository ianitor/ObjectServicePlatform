import {Injectable} from "@angular/core";
import {Apollo} from "apollo-angular";
import {
  FieldFilter,
  SearchFilter,
  Sort,
  SystemNotificationTemplateInput,
  SystemQueryInput, SystemUIPageInput,
  UpdateSystemNotificationTemplateInput,
  UpdateSystemQueryInput, UpdateSystemUIPageInput
} from "../graphQL/globalTypes";
import {Observable} from "rxjs";
import {NotificationTemplateDto} from "../models/notificationTemplateDto";
import {getNotificationTemplates, getNotificationTemplatesVariables} from "../graphQL/getNotificationTemplates";

import GetNotificationTemplateDetailsQuery from "../graphQL/getNotificationTemplateDetails.graphql";
import GetNotificationTemplatesQuery from './../graphQL/getNotificationTemplates.graphql';
import CreateNotificationTemplateMutation from "../graphQL/createNotificationTemplate.graphql";
import UpdateNotificationTemplateMutation from "../graphQL/updateNotificationTemplate.graphql";
import DeleteNotificationTemplateMutation from "../graphQL/deleteNotificationTemplate.graphql";

import GetCkEntitiesQuery from "../graphQL/getCkEntities.graphql";
import GetCkEntityDetailsQuery from "../graphQL/getCkEntityDetails.graphql";

import GetCkAttributesQuery from "../graphQL/getCkAttributes.graphql";

import GetRuntimeEntitiesQuery from "../graphQL/getRuntimeEntities.graphql";

import GetCkEntityAttributeQuery from "../graphQL/getCkEntityAttribute.graphql";

import GetQueryDetailsQuery from "../graphQL/getQueryDetails.graphql";
import GetCkAttributeDetailsQuery from "../graphQL/getCkAttributeDetails.graphql";
import GetQueriesQuery from "../graphQL/getQueries.graphql";
import CreateQueryMutation from "../graphQL/createQuery.graphql";
import UpdateQueryDetailsMutation from "../graphQL/updateQueryDetails.graphql";
import DeleteQueryMutation from "../graphQL/deleteQuery.graphql";

import GetServiceHooksQuery from "../graphQL/getServiceHooks.graphql";
import GetServiceHookDetailsQuery from "../graphQL/getServiceHookDetails.graphql";
import CreateServiceHookMutation from "../graphQL/createServiceHook.graphql";
import UpdateServiceHookMutation from "../graphQL/updateServiceHook.graphql";
import DeleteServiceHookMutation from "../graphQL/deleteServiceHook.graphql";

import GetPagesQuery from "../graphQL/getPages.graphql";
import GetPageDetailsQuery from "../graphQL/getPageDetails.graphql";
import CreatePageMutation from "../graphQL/createPage.graphql";
import UpdatePageMutation from "../graphQL/updatePage.graphql";
import DeletePageMutation from "../graphQL/deletePage.graphql";

import {NotificationTemplateDetailsDto} from "../models/notificationTemplateDetailsDto";
import {
  createNotificationTemplate,
  createNotificationTemplateVariables
} from "../graphQL/createNotificationTemplate";
import {
  updateNotificationTemplate,
  updateNotificationTemplateVariables
} from "../graphQL/updateNotificationTemplate";
import {
  getNotificationTemplateDetails,
  getNotificationTemplateDetailsVariables
} from "../graphQL/getNotificationTemplateDetails";
import {CkEntityDto} from "../models/ckEntityDto";
import {CkEntityDetailDto} from "../models/ckEntityDetailDto";
import {CkAttributeDto} from "../models/ckAttributeDto";
import {getCkAttributes, getCkAttributesVariables} from "../graphQL/getCkAttributes";
import {getCkEntities, getCkEntitiesVariables} from "../graphQL/getCkEntities";
import {getCkEntityDetails, getCkEntityDetailsVariables} from "../graphQL/getCkEntityDetails";
import {CkAttributeDetailDto} from "../models/ckAttributeDetailDto";
import {getCkAttributeDetails, getCkAttributeDetailsVariables} from "../graphQL/getCkAttributeDetails";
import {CkEntityAttributeDetailDto} from "../models/ckEntityAttributeDetailDto";
import {getCkEntityAttribute, getCkEntityAttributeVariables} from "../graphQL/getCkEntityAttribute";
import {getRuntimeEntities, getRuntimeEntitiesVariables} from "../graphQL/getRuntimeEntities";
import {RtEntityDto} from "../models/rtEntityDto";
import {RtEntityAttributeDto} from "../models/rtEntityAttributeDto";
import {IDictionary} from "../models/dictionary";
import {QueryDto} from "../models/queryDto";
import {QueryDetailsDto} from "../models/queryDetailsDto";
import {createQuery, createQueryVariables} from "../graphQL/createQuery";
import {updateQueryDetails, updateQueryDetailsVariables} from "../graphQL/updateQueryDetails";
import {deleteQuery, deleteQueryVariables} from "../graphQL/deleteQuery";
import {getQueries, getQueriesVariables} from "../graphQL/getQueries";
import {getQueryDetails, getQueryDetailsVariables} from "../graphQL/getQueryDetails";
import {ServiceHookDto} from "../models/serviceHookDto";
import {getServiceHooks, getServiceHooksVariables} from "../graphQL/getServiceHooks";
import {ServiceHookDetailsDto} from "../models/serviceHookDetailsDto";
import {getServiceHookDetails, getServiceHookDetailsVariables} from "../graphQL/getServiceHookDetails";
import {deleteServiceHook, deleteServiceHookVariables} from "../graphQL/deleteServiceHook";
import {updateServiceHook, updateServiceHookVariables} from "../graphQL/updateServiceHook";
import {createServiceHook, createServiceHookVariables} from "../graphQL/createServiceHook";
import {GraphQL, GraphQLCloneIgnoredProperties, OspGraphQLServiceBase, OspServiceOptions} from "@ianitor/osp-services";
import {PageDto} from "../models/pageDto";
import {deletePage, deletePageVariables} from "../graphQL/deletePage";
import {PageDetailsDto} from "../models/pageDetailsDto";
import {updatePage, updatePageVariables} from "../graphQL/updatePage";
import {createPage, createPageVariables} from "../graphQL/createPage";
import {getPageDetails, getPageDetailsVariables} from "../graphQL/getPageDetails";
import {getPages, getPagesVariables} from "../graphQL/getPages";
import {deleteNotificationTemplate, deleteNotificationTemplateVariables} from "../graphQL/deleteNotificationTemplate";
import {ObjectCloner, PagedResultDto} from "@ianitor/shared-services";
import {HttpLink} from "apollo-angular/http";

@Injectable({
  providedIn: 'root'
})
export class TenantBackendService extends OspGraphQLServiceBase {


  constructor(apollo: Apollo, httpLink: HttpLink, ospServiceOptions: OspServiceOptions) {
    super(apollo, httpLink, ospServiceOptions);

  }

  getNotificationTemplates(tenantId: string, skip: number = null, take: number = null, searchFilter: SearchFilter = null, sort: Sort[] = null): Observable<PagedResultDto<NotificationTemplateDto>> {

    const variables = <getNotificationTemplatesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getNotificationTemplates, NotificationTemplateDto,
      getNotificationTemplatesVariables>(tenantId, variables, GetNotificationTemplatesQuery, true,
      (resultSet, result) => {
        resultSet.totalCount = result.systemNotificationTemplateConnection.totalCount;
        resultSet.list = result.systemNotificationTemplateConnection.items;
      });
  }

  getNotificationTemplateDetails(tenantId: string, rtId: string): Observable<NotificationTemplateDetailsDto> {

    const variables = <getNotificationTemplateDetailsVariables>{
      rtId: rtId,
    };

    return this.getEntityDetail<getNotificationTemplateDetails, NotificationTemplateDetailsDto,
      getNotificationTemplateDetailsVariables>(tenantId, variables, GetNotificationTemplateDetailsQuery, true,
      result => result.systemNotificationTemplateConnection.items[0]);
  }

  createNotificationTemplate(tenantId: string, notificationTemplate: NotificationTemplateDetailsDto): Observable<NotificationTemplateDetailsDto> {

    const variables = <createNotificationTemplateVariables>{
      entities: [
        ObjectCloner.cloneObject<SystemNotificationTemplateInput, NotificationTemplateDto>(notificationTemplate, GraphQLCloneIgnoredProperties)
      ]
    };

    return this.createUpdateEntity<createNotificationTemplate, NotificationTemplateDetailsDto, createNotificationTemplateVariables>(tenantId,
      variables, CreateNotificationTemplateMutation, result => result.createSystemNotificationTemplates[0]);
  }

  updateNotificationTemplate(tenantId: string, notificationTemplate: NotificationTemplateDetailsDto): Observable<NotificationTemplateDetailsDto> {

    const variables = <updateNotificationTemplateVariables>{
      entities: [
        {
          rtId: notificationTemplate.rtId,
          item: ObjectCloner.cloneObject<UpdateSystemNotificationTemplateInput, NotificationTemplateDetailsDto>(notificationTemplate, GraphQLCloneIgnoredProperties)
        }
      ]
    };

    return this.createUpdateEntity<updateNotificationTemplate, NotificationTemplateDetailsDto, updateNotificationTemplateVariables>(tenantId,
      variables, UpdateNotificationTemplateMutation, result => result.updateSystemNotificationTemplates[0]);
  }

  deleteNotificationTemplate(tenantId: string, notificationTemplate: NotificationTemplateDto): Observable<boolean> {

    let variables = <deleteNotificationTemplateVariables>{
      entities: [
        {
          rtId: notificationTemplate.rtId
        }
      ]
    };

    return this.deleteEntity<deleteNotificationTemplate, deleteNotificationTemplateVariables>(tenantId, variables, DeleteNotificationTemplateMutation, result => result.deleteSystemNotificationTemplates);
  }

  getCkAttributes(tenantId: string, skip: number = null, take: number = null, searchFilter: SearchFilter = null, sort: Sort[] = null)
    : Observable<PagedResultDto<CkAttributeDto>> {

    const variables = <getCkAttributesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getCkAttributes, CkAttributeDto,
      getCkAttributesVariables>(tenantId, variables, GetCkAttributesQuery, true,
      (resultSet, result) => {
        resultSet.totalCount = result.constructionKitAttributes.totalCount;
        resultSet.list = result.constructionKitAttributes.items;
      });
  }

  getCkEntities(tenantId: string, skip: number = null, take: number = null, ckId: string = null, searchFilter: SearchFilter = null, sort: Sort[] = null): Observable<PagedResultDto<CkEntityDto>> {

    const variables = <getCkEntitiesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      ckId: ckId,
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getCkEntities, CkEntityDto,
      getCkEntitiesVariables>(tenantId, variables, GetCkEntitiesQuery, true,
      (resultSet, result) => {
        resultSet.totalCount = result.constructionKitTypes.totalCount;
        resultSet.list = result.constructionKitTypes.items;
      });
  }

  getCkEntityDetails(tenantId: string, ckId: string): Observable<CkEntityDetailDto> {

    const variables = <getCkEntityDetailsVariables>{
      ckId: ckId
    };

    return this.getEntityDetail<getCkEntityDetails, CkEntityDetailDto,
      getCkEntityDetailsVariables>(tenantId, variables, GetCkEntityDetailsQuery,true,
      result => {
        const t = <CkEntityDetailDto>ObjectCloner.cloneObject(result.constructionKitTypes.items[0], [...GraphQLCloneIgnoredProperties, 'attributes']);
        t.attributes = result.constructionKitTypes.items[0].attributes.items;
        t.derivedTypes = result.constructionKitTypes.items[0].derivedTypes.items;
        return t;
      });
  }


  getCkAttributeDetails(tenantId: string, attributeId: string): Observable<CkAttributeDetailDto> {

    const variables = <getCkAttributeDetailsVariables>{
      attributeId: attributeId
    };

    return this.getEntityDetail<getCkAttributeDetails, CkAttributeDetailDto,
      getCkAttributeDetailsVariables>(tenantId, variables, GetCkAttributeDetailsQuery,true,
      result => {
        return <CkAttributeDetailDto>ObjectCloner.cloneObject(result.constructionKitAttributes.items[0], GraphQLCloneIgnoredProperties);
      });
  }

  getCkEntityAttributeDetails(tenantId: string, ckId: string, attributeName: string): Observable<CkEntityAttributeDetailDto> {

    const variables = <getCkEntityAttributeVariables>{
      ckId: ckId,
      attributeName: attributeName
    };

    return this.getEntityDetail<getCkEntityAttribute, CkEntityAttributeDetailDto,
      getCkEntityAttributeVariables>(tenantId, variables, GetCkEntityAttributeQuery,true,
      result => {
        return <CkEntityAttributeDetailDto>ObjectCloner.cloneObject(result.constructionKitTypes.items[0].attributes.items[0], GraphQLCloneIgnoredProperties);
      });
  }

  getRtEntities(tenantId: string, ckId: string, attributeNames: Array<string>, skip: number = null, take: number = null, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[], sort: Sort[] = null): Observable<PagedResultDto<RtEntityDto>> {

    const variables = <getRuntimeEntitiesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      ckId: ckId,
      attributeNames: attributeNames,
      fieldFilters: fieldFilter,
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getRuntimeEntities, RtEntityDto,
      getRuntimeEntitiesVariables>(tenantId, variables, GetRuntimeEntitiesQuery, true,
      (resultSet, result) => {
        resultSet.totalCount = result.runtimeEntities.totalCount;
        resultSet.list = result.runtimeEntities.items.map(value => {

          const rtEntity = <RtEntityDto>{
            rtId: value.rtId,
            wellKnownName: value.wellKnownName,
            attributes: <IDictionary<RtEntityAttributeDto>>{}
          };

          value.attributes.items.forEach(attr => {
            rtEntity.attributes[attr.attributeName] = <RtEntityAttributeDto>{
              value: attr.value,
              values: attr.values
            }
          });

          return rtEntity;
        });
      });
  }


  getQueries(tenantId: string, skip: number = null, take: number = null, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null): Observable<PagedResultDto<QueryDto>> {

    const variables = <getQueriesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      fieldFilters: fieldFilter,
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getQueries, QueryDto, getQueriesVariables>(tenantId, variables, GetQueriesQuery, true, (resultSet, result) => {
      resultSet.totalCount = result.systemQueryConnection.totalCount;
      resultSet.list = result.systemQueryConnection.items;
    });
  }

  getQueryDetails(tenantId: string, rtId: string): Observable<QueryDetailsDto> {

    const variables = <getQueryDetailsVariables>{
      rtId: rtId
    };

    return this.getEntityDetail<getQueryDetails, QueryDetailsDto,
      getQueryDetailsVariables>(tenantId, variables, GetQueryDetailsQuery,true,
      result => {
        return <QueryDetailsDto>result.systemQueryConnection.items[0];
      });
  }

  createQuery(tenantId: string, queryDto: QueryDetailsDto): Observable<QueryDetailsDto> {

    const variables = <createQueryVariables>{
      entities: [
        ObjectCloner.cloneObject<SystemQueryInput, QueryDetailsDto>(queryDto, GraphQLCloneIgnoredProperties)
      ]
    };

    return this.createUpdateEntity<createQuery, QueryDetailsDto, createQueryVariables>(tenantId,
      variables, CreateQueryMutation, result => result.createSystemQuerys[0]);
  }

  updateQuery(tenantId: string, queryDto: QueryDetailsDto): Observable<QueryDetailsDto> {

    let variables = <updateQueryDetailsVariables>{
      entities: [
        {
          rtId: queryDto.rtId,
          item: ObjectCloner.cloneObject<UpdateSystemQueryInput, QueryDetailsDto>(queryDto, GraphQLCloneIgnoredProperties)
        }
      ]
    };

    return this.createUpdateEntity<updateQueryDetails, QueryDetailsDto, updateQueryDetailsVariables>(tenantId,
      variables, UpdateQueryDetailsMutation, result => result.updateSystemQuerys[0]);
  }

  deleteQuery(tenantId: string, queryDto: QueryDetailsDto): Observable<boolean> {

    let variables = <deleteQueryVariables>{
      entities: [
        {
          rtId: queryDto.rtId
        }
      ]
    };

    return this.deleteEntity<deleteQuery, deleteQueryVariables>(tenantId, variables, DeleteQueryMutation, result => result.deleteSystemQuerys);
  }

  getServiceHooks(tenantId: string, skip: number = null, take: number = null, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null): Observable<PagedResultDto<ServiceHookDto>> {

    const variables = <getServiceHooksVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      fieldFilters: fieldFilter,
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getServiceHooks, ServiceHookDto, getServiceHooksVariables>(tenantId, variables, GetServiceHooksQuery, true, (resultSet, result) => {
      resultSet.totalCount = result.systemServiceHookConnection.totalCount;
      resultSet.list = result.systemServiceHookConnection.items;
    });
  }

  getServiceHookDetails(tenantId: string, rtId: string): Observable<ServiceHookDetailsDto> {

    const variables = <getServiceHookDetailsVariables>{
      rtId: rtId
    };

    return this.getEntityDetail<getServiceHookDetails, ServiceHookDetailsDto,
      getServiceHookDetailsVariables>(tenantId, variables, GetServiceHookDetailsQuery,true,
      result => {
        return result.systemServiceHookConnection.items[0];
      });
  }


  createServiceHook(tenantId: string, serviceHookDetailsDto: ServiceHookDetailsDto): Observable<ServiceHookDetailsDto> {

    const variables = <createServiceHookVariables>{
      entities: [
        ObjectCloner.cloneObject<SystemQueryInput, ServiceHookDetailsDto>(serviceHookDetailsDto, GraphQLCloneIgnoredProperties)
      ]
    };

    return this.createUpdateEntity<createServiceHook, ServiceHookDetailsDto, createServiceHookVariables>(tenantId,
      variables, CreateServiceHookMutation, result => result.createSystemServiceHooks[0]);
  }

  updateServiceHook(tenantId: string, serviceHookDetailsDto: ServiceHookDetailsDto): Observable<ServiceHookDetailsDto> {

    let variables = <updateServiceHookVariables>{
      entities: [
        {
          rtId: serviceHookDetailsDto.rtId,
          item: ObjectCloner.cloneObject<UpdateSystemQueryInput, ServiceHookDetailsDto>(serviceHookDetailsDto, GraphQLCloneIgnoredProperties)
        }
      ]
    };

    return this.createUpdateEntity<updateServiceHook, ServiceHookDetailsDto, updateServiceHookVariables>(tenantId,
      variables, UpdateServiceHookMutation, result => result.updateSystemServiceHooks[0]);
  }

  deleteServiceHook(tenantId: string, serviceHookDto: ServiceHookDto): Observable<boolean> {

    let variables = <deleteServiceHookVariables>{
      entities: [
        {
          rtId: serviceHookDto.rtId
        }
      ]
    };

    return this.deleteEntity<deleteServiceHook, deleteServiceHookVariables>(tenantId, variables, DeleteServiceHookMutation, result => result.deleteSystemServiceHooks);
  }

  getPages(tenantId: string, skip: number = null, take: number = null, searchFilter: SearchFilter = null, fieldFilter: FieldFilter[] = null, sort: Sort[] = null): Observable<PagedResultDto<PageDto>> {

    const variables = <getPagesVariables>{
      first: take,
      after: GraphQL.offsetToCursor(skip),
      fieldFilters: fieldFilter,
      searchFilter: searchFilter,
      sort: sort
    };

    return this.getEntities<getPages, PageDto, getPagesVariables>(tenantId, variables, GetPagesQuery, true, (resultSet, result) => {
      resultSet.totalCount = result.systemUIPageConnection.totalCount;
      resultSet.list = result.systemUIPageConnection.items;
    });
  }

  getPageDetails(tenantId: string, rtId: string): Observable<PageDetailsDto> {

    const variables = <getPageDetailsVariables>{
      rtId: rtId
    };

    return this.getEntityDetail<getPageDetails, PageDetailsDto,
      getPageDetailsVariables>(tenantId, variables, GetPageDetailsQuery,true,
      result => {
        return result.systemUIPageConnection.items[0];
      });
  }


  createPage(tenantId: string, pageDetailsDto: PageDetailsDto): Observable<PageDetailsDto> {

    const variables = <createPageVariables>{
      entities: [
        ObjectCloner.cloneObject<SystemUIPageInput, PageDetailsDto>(pageDetailsDto, GraphQLCloneIgnoredProperties)
      ]
    };

    return this.createUpdateEntity<createPage, PageDetailsDto, createPageVariables>(tenantId,
      variables, CreatePageMutation, result => result.createSystemUIPages[0]);
  }

  updatePage(tenantId: string, pageDetailsDto: PageDetailsDto): Observable<PageDetailsDto> {

    let variables = <updatePageVariables>{
      entities: [
        {
          rtId: pageDetailsDto.rtId,
          item: ObjectCloner.cloneObject<UpdateSystemUIPageInput, PageDetailsDto>(pageDetailsDto, GraphQLCloneIgnoredProperties)
        }
      ]
    };

    return this.createUpdateEntity<updatePage, PageDetailsDto, updatePageVariables>(tenantId,
      variables, UpdatePageMutation, result => result.updateSystemUIPages[0]);
  }

  deletePage(tenantId: string, pageDto: PageDto): Observable<boolean> {

    let variables = <deletePageVariables>{
      entities: [
        {
          rtId: pageDto.rtId
        }
      ]
    };

    return this.deleteEntity<deletePage, deletePageVariables>(tenantId, variables, DeletePageMutation, result => result.deleteSystemUIPages);
  }
}
