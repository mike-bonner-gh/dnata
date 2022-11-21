# dnata

First version of simple movies API.
This allows consumers to:
- Search for movies using either Title, Year of Release & Genres
- Query top 5 movies based upon the total user ratings
- Query top 5 movies based upon a specific users rating.
- Add or update the rating for a movie for an individual user.

Assumptions:
- This is a backend service, therefore authentication and authorization will be handled by another service.

Improvements.
- Authentication and Authorization should be added, especially at user level operations.
- Currently all data is loaded from json files into memory. This doesn\'t currently affect performance as the amount of data is limited. 
A future version should limit the amount of data which is returned at the repository level to mitigate performance issues.
- There is some data selection logic in the repository which isn\'t currently unit tested. This needs to be moved or tests need to be added.#
- The unit tests could be grouped to limit the amount of data setup which is needed.
