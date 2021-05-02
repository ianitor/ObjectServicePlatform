print("Creating users");


var user = {
    user: "osp-system-admin",
    pwd: "OspAdmin1", // passwordPrompt(),      // Or  "<cleartext password>"
    roles: [
        { role: "userAdminAnyDatabase", db: "admin" },
        { role: "userAdmin", db: "admin" },
        { role: "readWrite", db: "admin" },
        { role: "dbAdmin", db: "admin" },
        { role: "clusterAdmin", db: "admin" },
        { role: "readWriteAnyDatabase", db: "admin" },
        { role: "dbAdminAnyDatabase", db: "admin" },
    ]
}
db.createUser(user)
