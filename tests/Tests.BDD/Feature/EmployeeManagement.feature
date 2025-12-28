# language: en
Feature: Employee management
  As a system administrator
  I want to manage organization employees
  So that access to the system is controlled

  Scenario: Register employee successfully
    Given I have the data of a new employee:
      | Field     | Value                |
      | Cpf       | 43834548014          |
      | Name      | Ana                  |
      | Surname   | Souza                |
      | Email     | ana.souza@empresa.com|
      | Password  | Senha@123            |
    When I request the employee registration
    Then the employee should be persisted with the encrypted password
    And the system should return the data for "Ana" with a generated ID

  Scenario: Attempt to remove non-existent employee
    Given there is no employee with ID 500
    When I request deletion of employee 500
    Then the system should report that 0 records were affected

  Scenario: Error when registering with empty CPF
    Given I have the data of a new employee:
      | Field     | Value                |
      | Cpf       |                      |
      | Name      | Error                |
      | Password  | Senha@123            |
    When I request the employee registration
    Then the system should return an error containing the message "CPF"
