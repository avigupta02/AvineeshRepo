Feature: Worker management
  Scenario: Add and view a worker
    When I create a valid worker
    Then the response is successful
    And the worker appears in the collection
  Scenario: Update and delete a worker
    Given an existing worker
    When I update the worker
    Then the updated values are returned
    When I delete the worker
    Then the worker is not found
  Scenario: Reject invalid input
    When I submit an invalid worker
    Then the response status is 400
