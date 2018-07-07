## Some Context

Neo4jClient just gives you a nice way to execute Cypher commands against a Neo4j instance. You always need to start with a working Cypher query, then you can write it in C#.

This page just shows some examples of how to translate different Cypher queries into C#.

For more explanation about how we handle parameters, immutable query objects, custom return clauses and things like that, you should really take the time to read the main [Neo4jClient Cypher documentation](cypher) that we have published. This page is just examples.

## Neo4j Versions

Most of the examples on this page are written with Neo4j 2.0 in mind, so they skip the `START` clause, and use clauses like `MERGE`. The focus of this page is about Cypher-to-C# syntax though, and should be equally useful in helping you translate a Neo4j 1.9 query to C#.

At the end of the day, you always need to start with a working Cypher query, *then* work out the equivalent C#.

## Need more help?

If you have a working Cypher query, but can't translate it to C#, just post it on [[http://stackoverflow.com/questions/tagged/neo4jclient|on StackOverflow using the neo4jclient tag]] and we'll help you out pretty quickly.

Then, once we have the answer, we can add it to this page too so it helps other people.

## User class

Most of the examples below assume you have the following class, to represent the structure of a user node:

    public class User
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }

## Get all users by label

This Cypher:

    MATCH (user:User)
    RETURN user

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Return(user => user.As<User>())
        .Results

## Get specific user

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 1234
    RETURN user

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 1234)
        .Return(user => user.As<User>())
        .Results

## Get a user, and the count of their friends

This Cypher:

    OPTIONAL MATCH (user:User)-[FRIENDS_WITH]-(friend:User)
    WHERE user.Id = 1234
    RETURN user, count(friend) AS NumberOfFriends

Is this C#:

    graphClient.Cypher
        .OptionalMatch("(user:User)-[FRIENDS_WITH]-(friend:User)")
        .Where((User user) => user.Id == 1234)
        .Return((user, friend) => new {
            User = user.As<User>(),
            NumberOfFriends = friend.Count()
        })
        .Results

## Get a user, and all their friends

This Cypher:

    OPTIONAL MATCH (user:User)-[FRIENDS_WITH]-(friend:User)
    WHERE user.Id = 1234
    RETURN user, collect(friend) AS NumberOfFriends

Is this C#:

    graphClient.Cypher
        .OptionalMatch("(user:User)-[FRIENDS_WITH]-(friend:User)")
        .Where((User user) => user.Id == 1234)
        .Return((user, friend) => new {
            User = user.As<User>(),
            Friends = friend.CollectAs<User>()
        })
        .Results

## Create a user

This Cypher:

    CREATE (user:User { Id: 456, Name: 'Jim' })

Should use parameters:

    CREATE (user:User {newUser})

And is this C#:

    var newUser = new User { Id = 456, Name = "Jim" };
    graphClient.Cypher
        .Create("(user:User {newUser})")
        .WithParam("newUser", newUser)
        .ExecuteWithoutResults();

Note that we're using an explicitly named parameter (`newUser`) and the query, and the `WithParam` method to supply it. This keeps our encoding safe, protects against Cypher-injection attacks, and improves performance by allowing query plans to be cached.

## Create a user, only if they don't already exist

This Cypher:

    MERGE (user:User { Id: 456 })
    ON CREATE user
    SET user.Name = 'Jim'

Should use parameters:

    CREATE (user:User { Id: {id} })
    ON CREATE user
    SET user = {newUser}

And is this C#:

    var newUser = new User { Id = 456, Name = "Jim" };
    graphClient.Cypher
        .Merge("(user:User { Id: {id} })")
        .OnCreate()
        .Set("user = {newUser}")
        .WithParams(new {
            id = newUser.Id,
            newUser
        })
        .ExecuteWithoutResults();

## Create a user and relate them to an existing one

This Cypher:

    MATCH (invitee:User)
    WHERE invitee.Id = 123
    CREATE (invitee)-[:INVITED]->(invited:User {newUser})

Is this C#:

    var newUser = new User { Id = 456, Name = "Jim" };
    graphClient.Cypher
        .Match("(invitee:User)")
        .Where((User invitee) => invitee.Id == 123)
        .Create("(invitee)-[:INVITED]->(invited:User {newUser})")
        .WithParam("newUser", newUser)
        .ExecuteWithoutResults();

## Relate two existing users

This Cypher:

    MATCH (user1:User), (user2:User)
    WHERE user1.Id = 123, user2.Id = 456
    CREATE user1-[:FRIENDS_WITH]->user2

Is this C#:

    graphClient.Cypher
        .Match("(user1:User)", "(user2:User)")
        .Where((User user1) => user1.Id == 123)
        .AndWhere((User user2) => user2.Id == 456)
        .Create("user1-[:FRIENDS_WITH]->user2")
        .ExecuteWithoutResults();

## Relate two existing users, only if they aren't related already

This Cypher:

    MATCH (user1:User), (user2:User)
    WHERE user1.Id = 123, user2.Id = 456
    CREATE UNIQUE user1-[:FRIENDS_WITH]->user2

Is this C#:

    graphClient.Cypher
        .Match("(user1:User)", "(user2:User)")
        .Where((User user1) => user1.Id == 123)
        .AndWhere((User user2) => user2.Id == 456)
        .CreateUnique("user1-[:FRIENDS_WITH]->user2")
        .ExecuteWithoutResults();

## Update a single property on a user

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 123
    SET user.Age = 25

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 123)
        .Set("user.Age = {age}")
        .WithParam("age", 25)
        .ExecuteWithoutResults();

Note: we're using parameters again to pass in data. **Never** do this via string concatenation like `Set("user.Age = " + age.ToString())` otherwise you will introduce encoding bugs, security risks, and significantly impact your query performance by bypassing the query plan cache in Neo4j itself.

## Replace all the properties on a user

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 123
    SET user = { Id: 123, Age: 25, Email: 'tatham@oddie.com.au' }

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 123)
        .Set("user = {tatham}")
        .WithParam("tatham", new User { Id = 123, Age = 25, Email = "tatham@oddie.com.au" })
        .ExecuteWithoutResults();

## Delete a user

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 123
    DELETE user

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 123)
        .Delete("user")
        .ExecuteWithoutResults();

## Delete a user and all inbound relationships

This Cypher:

    OPTIONAL MATCH (user:User)<-[r]-()
    WHERE user.Id = 123
    DELETE r, user

Is this C#:

    graphClient.Cypher
        .OptionalMatch("(user:User)<-[r]-()")
        .Where((User user) => user.Id == 123)
        .Delete("r, user")
        .ExecuteWithoutResults();

## Get all labels for a specific user

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 1234
    RETURN labels(user)

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 1234)
        .Return(user => user.Labels())
        .Results

## Get all labels for a specific user, and still the user too

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 1234
    RETURN user, labels(user)

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 1234)
        .Return(user => new {
            User = user.As<User>(),
            Labels = user.Labels()
        })
        .Results

## Get a user, count their friends then add this number to the user and return. 

Note: This is an example of using Neo4j 3.0 Stored Procedures. There are other ways of adding a property to an object, this is just an example of CALL and YIELD, using [apoc Neo4j Stored Procedures](https://github.com/neo4j-contrib/neo4j-apoc-procedures)

This Cypher:

    MATCH (user:User)
    WHERE user.Id = 1234
    WITH user, size((user)-[:IS_FRIENDS_WITH]->(:Friend)) as numberOfFriends
    CALL apoc.map.setKey(user, 'numberOfFriends', numberOfFriends) YIELD value AS userWithFriends
    RETURN userWithFriends

Is this C#:

    graphClient.Cypher
        .Match("(user:User)")
        .Where((User user) => user.Id == 1234)
        .With("user, size((user)-[:IS_FRIENDS_WITH]->(:Friend)) as numberOfFriends")
        .Call("apoc.map.setKey(user, 'numberOfFriends', numberOfFriends)").Yield("value AS userWithFriends")
        .Return(userWithFriends => new {
            User = userWithFriends.As<User>()
        })
        .Results
