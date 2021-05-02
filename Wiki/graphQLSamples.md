
# Query

## CK
~~~~
{
  constructionKitTypes {
    items {
      entityIdKey
      baseType {
        entityIdKey
      }
      revision
      scopeId
      derivedTypes {
        items {
          entityIdKey
        }
      }
      attributes {
        items {
          attributeId
          attributeName
          attributeValueType
          defaultValues {
            ...comparisonFields
          }
          defaultValue {
            ...comparisonFields
          }
        }
      }
    }
  }
}

fragment comparisonFields on ValueUnionType {
  __typename
  ... on StringValue {
    value
  }
  ... on Int32Value {
    value
  }
}
~~~~
## RT
~~~~
query dtest {
  paketServiceContactConnection(rtId: "b50ee731-5655-46ed-b19a-9d357ac2eb84") {
    items {
      id
      revision
      rtId
      firstName
      lastName
      street
      city
      children {
        totalCount
        edges {
          cursor
          node {
            id
            sender
          }
        }
        pageInfo {
          endCursor
          hasNextPage
          startCursor
          hasPreviousPage
        }
        items {
          id
          revision
          rtId
          sender
          deliveryService
        }
      }
    }
  }
}
~~~~
# Mutations

## Create
~~~~
mutation($entities: [PaketServiceContactInput]!) {
  createPaketServiceContacts(entities: $entities) {   
    id
    revision
    rtId
    ckEntityId
  }
}
~~~~
### Query Variables
~~~~
{"entities": [
  {"wellKnownName": "test", "firstName": "gerald", "lastName": "Lochner", "street": "Firmianstraße 31a", "children": [
    { "modOption" : "CREATE", "targetInstanceId": "0b5561a8-962f-463f-adfd-eef00293e194"}
  ]
  },
  {"wellKnownName": "test", "firstName": "Marlene", "lastName": "Lochner", "street": "Firmianstraße 31A"}
]
} 
~~~~
## Update
~~~~
mutation($entities: [UpdatePaketServiceContactInput]!) {
  updatePaketServiceContacts(entities:$entities) {
    id
    rtId
    revision
    ckEntityId
    street
  }
}
~~~~

### Query Variables
~~~~
{"entities": [
  {"rtId": "b50ee731-5655-46ed-b19a-9d357ac2eb84", "revision": "_Y1NWfea--_", "item":
    { "firstName": "gerald", "lastName": "Lochner", "street": "Zeller Fusch 153", "children":
      [
     	 { "targetInstanceId":"0b5561a8-962f-463f-adfd-eef00293e194", "modOption":"DELETE"}
      ]
    }
  },
  {"rtId": "7a851452-b05c-47a1-8b69-452806d707e2", "revision": "_Y1NWfea--B", "item": { "firstName": "Marlene", "lastName": "Lochner", "street": "Zeller Fusch 153"}}
]
} 
~~~~

## Delete
~~~~
mutation($entities: [DeletionPaketServiceContactInput]!) {
  deletePaketServiceContacts(entities:$entities) 
}
~~~~

### Query Variables
~~~~
{"entities": [
  {"rtId": "39e8c978-6773-4e4f-a4d2-dd5d9f224398", "revision": "_Y0FHBSi--_"},
  {"rtId": "c0e6c9d1-44ff-4660-bf18-89c8fe47ba6f", "revision": "_Y0FHBZ2--_"}
]
} 
~~~~
# Subscription
~~~~
subscription PaketServiceContactUpdated($id: String) {
  paketServiceContactUpdated(rtId: $id, updateTypes: [ADDED, UPDATED, DELETED]) {
    items {
      item {
        id
        revision
        firstName
        lastName
        street
      }
      updateState
    }
  }
}
~~~~
### Query Variables
~~~~
{"entities": [
  {"rtId": "39e8c978-6773-4e4f-a4d2-dd5d9f224398", "revision": "_Y0FHBSi--_"},
  {"rtId": "c0e6c9d1-44ff-4660-bf18-89c8fe47ba6f", "revision": "_Y0FHBZ2--_"}
]
} 
~~~~
# Client

## Code generation

Install
~~~~
npm install -g apollo-codegen
~~~~

Fetch schema
~~~~
apollo-codegen introspect-schema https://localhost:5001/dataSources/paketservice/GraphQL --insecure --output schema.json 
~~~~

Generate TypeScript 
~~~~
apollo-codegen generate **/*.graphql --schema schema.json --target typescript --output operation-result-types.ts
~~~~

If there is an error message like: 
~~~~
Error: Cannot use GraphQLInputObjectType "SystemNotificationTemplateInput" from another module or realm.
Ensure that there is only one instance of "graphql" in the node_modules
directory. If different versions of "graphql" are the dependencies of other
relied on modules, use "resolutions" to ensure only one version is installed.
https://yarnpkg.com/en/docs/selective-version-resolutions
~~~~

try to
~~~~
remove-item node_modules -force -recurse
npm install
npm dedupe
~~~~