/*
// <copyright company="Intuit">
// Author:Nimisha
//
*/
--Delete data from OAuthTokens table if any
delete form dbo.OAuthTokens

--Insert the data that need to be synced
insert into dbo.OAuthTokens values('123145693359857','2016-05-06','qyprdFBgt5mkWX6nrsqtKPgKgzeB3rqTGwqANpV8YOnCWUK9','XaZCEi6T6J1s689RfEEsDDtQe5ASfk5LbaNqFILQ','QBO' );
insert into dbo.OAuthTokens values('1269959970','2016-05-06','qyprdIyga7rdFS9Oe9IyXWZqqs6cYhfXUmR4iQD6XqX3iIrU','PYly9uD6DB8togTDfiqrOytMxafp2udvA5ds1qFV','QBO' );