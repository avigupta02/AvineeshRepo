Feature: Worker Management API
  As an administrator
  I want to manage workers through the API
  So that the worker directory remains complete, persistent, and accurate

  Scenario: Accept a valid worker payload
    Given the Worker Management API is available
    When I submit a worker with WorkerId "W-100", Name "Ada Lovelace", Email "ada@example.com", Department "Engineering", and Designation "Engineer"
    Then the worker payload is accepted
    And the worker has a unique WorkerId

  Scenario Outline: Reject invalid worker data without persisting it
    Given the Worker Management API is available
    When I submit a worker with <invalid data>
    Then the API returns a client-readable validation response
    And the worker is not persisted
    Examples:
      | invalid data |
      | a missing required field |
      | a malformed email address |

  Scenario: Reject a worker with a duplicate WorkerId
    Given a worker with WorkerId "W-100" is already persisted
    When I submit another valid worker with WorkerId "W-100"
    Then the API rejects the duplicate identifier with a client-readable response
    And the new worker is not persisted

  Scenario: Create and list a persisted worker
    Given the configured JSON data file contains no worker with WorkerId "W-101"
    When I create a valid worker with WorkerId "W-101"
    Then the API identifies the created resource
    And the worker is serialized to the configured JSON data file
    When the API restarts and I request the worker collection
    Then the persisted worker and its fields are returned

  Scenario: Return an empty collection when no workers exist
    Given no workers are persisted
    When I request the worker collection
    Then the API returns an empty collection
    And the request is not treated as an error

  Scenario: Update an existing worker
    Given a worker with WorkerId "W-102" exists
    When I update the worker with valid changes
    Then the API reflects the new worker values
    And the updated worker is persisted

  Scenario: Return not found when updating an unknown worker
    Given no worker with WorkerId "W-404" exists
    When I update worker "W-404"
    Then the API returns a not-found response
    And no worker is created

  Scenario: Delete an existing worker
    Given a worker with WorkerId "W-103" exists
    When I delete worker "W-103"
    Then the worker is removed from JSON storage
    And subsequent worker reads do not return "W-103"

  Scenario: Return not found when deleting an unknown worker
    Given no worker with WorkerId "W-405" exists
    And other workers are persisted
    When I delete worker "W-405"
    Then the API returns a not-found response
    And the other workers remain unchanged

  Scenario: Preserve worker data when JSON file access fails
    Given workers are persisted in the configured JSON data file
    And a file access or serialization error occurs during a write
    When a worker write operation is requested
    Then the API returns a safe error response
    And existing persisted worker data is not corrupted

  Scenario: Discover worker CRUD endpoints through Swagger UI
    Given the API is running
    When I open Swagger UI
    Then all worker CRUD endpoints and request and response schemas are discoverable
    And I can execute a documented endpoint with valid data

  Scenario: Return a safe response for an unexpected exception
    Given an unexpected application or persistence exception occurs
    When I call a worker endpoint
    Then centralized error handling returns a meaningful error response
    And the response does not expose sensitive implementation details

  Scenario Outline: Return an appropriate error for an invalid API request
    Given the API is running
    When I request <invalid request>
    Then the API returns an appropriate HTTP error status
    Examples:
      | invalid request |
      | a malformed route |
      | an unsupported operation |

  Scenario: Resolve dependencies through the three application layers
    Given a new checkout of the solution
    When the solution is built and the API starts
    Then all projects restore and compile on .NET 10
    And API, business, and data-access dependencies are registered through dependency injection
    And the request invokes business logic instead of persistence rules in a controller

  Scenario: Report automated coverage for CRUD validation and errors
    Given the business and data-access components and the BDD test project
    When the automated test command completes
    Then core CRUD, validation, error, add, view, update, and delete behaviors are covered
    And test results identify any failing scenario or test
