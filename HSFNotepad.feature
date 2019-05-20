Feature: HSF Sample Automation
  I want to use this template for my feature file

  @tag1
  Scenario: Save a note in notepad
    Given I want to save a note in noptepad
    When  I open a note pad
    And   I create a new note
#    And   I save it on local path
#    Then  I should see a note created
#
#  @tag2
#  Scenario Outline: Save a note in notepad with Name
#    Given I want to save a note in noptepad
#    When  I open a note pad
#    And   I create a new note with '<name>' and '<text>'
#    And   I save it on local '<path>'
#    Then  I should see a note created
#
#    Examples: 
#      | name  | text                | path       |
#      | name1 | this notepad file 1 | D:\NotePads|
#      | name2 | this notepad file 7 | D:\NotePads|
