import {DocumentNode} from "graphql";
import {finalize, map} from "rxjs/operators";
import {Apollo} from "apollo-angular";
import {OspServiceOptions} from "../options/osp-serviceOptions";
import {PagedGraphResultDto} from "../models/pagedGraphResultDto";
import {PagedResultDto} from "@ianitor/shared-services";
import {HttpLink} from 'apollo-angular/http';
import {InMemoryCache} from '@apollo/client/core';

export class OspGraphQLServiceBase {

  constructor(private apollo: Apollo, private httpLink: HttpLink, private ospServiceOptions: OspServiceOptions) {
  }

  private createApolloForTenant(tenantId: string) {

    const result = this.apollo.use(tenantId);
    if (result) {
      return;
    }

    const uri = `${this.ospServiceOptions.coreServices}tenants/${tenantId}/GraphQL`;

    this.apollo.createNamed(tenantId,
      {
        link: this.httpLink.create({uri}),
        cache: new InMemoryCache({
          dataIdFromObject: o => <string>o.rtId
        }),
      });
  }

  private prepareWatchQuery<TResult, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode) {
    this.createApolloForTenant(tenantId);

    return this.apollo.use(tenantId).watchQuery<TResult>({
      query: queryNode,
      variables: variables
    }).valueChanges;
  }


  private prepareQuery<TResult, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode) {
    this.createApolloForTenant(tenantId);

    return this.apollo.use(tenantId).query<TResult>({
      query: queryNode,
      variables: variables,
      fetchPolicy: "network-only"
    });
  }


  protected getEntities<TResult, TEntity, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode, watchQuery: boolean, f: (resultSet: PagedResultDto<TEntity>, result: TResult) => void) {

    const query = watchQuery ? this.prepareWatchQuery<TResult, TVariable>(tenantId, variables, queryNode) : this.prepareQuery<TResult, TVariable>(tenantId, variables, queryNode);
    return query.pipe(map(result => {

      const resultSet = new PagedResultDto<TEntity>();

      if (result.errors) {
        console.error(result.errors);
        throw Error("Error in GraphQL statement.")
      } else if (result.data) {
        f(resultSet, result.data);
      }
      return resultSet;
    }));
  }

  protected getGraphEntities<TResult, TP, TC, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode, watchQuery: boolean, f: (resultSet: PagedGraphResultDto<TP, TC>, result: TResult) => void) {

    const query = watchQuery ? this.prepareWatchQuery<TResult, TVariable>(tenantId, variables, queryNode) : this.prepareQuery<TResult, TVariable>(tenantId, variables, queryNode);
    return query.pipe(map(result => {

      const resultSet = new PagedGraphResultDto<TP, TC>();

      if (result.errors) {
        console.error(result.errors);
        throw Error("Error in GraphQL statement.")
      } else if (result.data) {
        f(resultSet, result.data);
      }
      return resultSet;
    }));
  }

  protected getEntityDetail<TResult, TEntity, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode, watchQuery: boolean, f: (result: TResult) => TEntity) {

    const query = watchQuery ? this.prepareWatchQuery<TResult, TVariable>(tenantId, variables, queryNode) : this.prepareQuery<TResult, TVariable>(tenantId, variables, queryNode);
    return query.pipe(map(result => {

      if (result.errors) {
        console.error(result.errors);
        throw Error("Error in GraphQL statement.")
      } else if (result.data) {
        return f(result.data);
      }
      return null;
    }));
  }

  protected createUpdateEntity<TResult, TEntity, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode, f: (result: TResult) => TEntity) {

    this.createApolloForTenant(tenantId);

    return this.apollo.use(tenantId).mutate<TResult>({
      mutation: queryNode,
      variables: variables
    }).pipe(
      map(value => f(value.data)),
      finalize(() => this.apollo.use(tenantId).getClient().reFetchObservableQueries(true))
    );
  }

  protected deleteEntity<TResult, TVariable>(tenantId: string, variables: TVariable, queryNode: DocumentNode, f: (result: TResult) => boolean) {
    this.createApolloForTenant(tenantId);

    return this.apollo.use(tenantId).mutate<TResult>({
      mutation: queryNode,
      variables: variables
    }).pipe(
      map(value => f(value.data)),
      finalize(() => this.apollo.use(tenantId).getClient().reFetchObservableQueries(true))
    );
  }


}
