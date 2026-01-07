# language: en
Feature: Customer management
  As a system analyst
  I want to perform CRUD operations on customers
  So the user base can be kept up to date in the database

  Background: Clean database
    Given the persistence system is operational

  Scenario: Register new customer successfully
    Given I filled the data of a new customer with CPF "43834548014" and name "Carlos"
    When I confirm the registration
    Then the customer should be saved in the database
    And the system should return a DTO with the name "Carlos" and a valid ID

  Scenario: Update non-existent customer
    Given there is no customer registered with ID 999
    When I try to update customer 999 to name "Roberto"
    Then the system should return an error with message "Customer not found."
