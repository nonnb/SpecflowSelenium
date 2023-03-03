Feature: Balance Transfer

Scenario: Transfering to a brand new account reflects the transferred amount
	Given I create a new 'Savings' Bank Account for account holder 'Joe Bloggs'
	When I transfer 200 from Account '00001' to this account
	Then this Account balance should be 200

Scenario: When I type random stuff into the Amount, I should get a Validation Error
	Given I attempt transfer of funds from account '00001'
	When I type '<badAmount>' into the 'Amount' input
	Then I should see the validation error 'The field Amount must be a number'
	Examples:
	| badAmount                  |
	| :)                         |
	| 1.2.3.4.5                  |
	| '; drop table 'Account' -- |