# Stackoverflow Tag API

This repository contains a .NET Core API for managing Stackoverflow tags.

## Tags Controller

The `TagsController` manages tags within the API.

### Endpoints

#### Get Tags

- **URL:** `/tags`
- **Method:** `GET`
- **Description:** Retrieves a list of tags.
- **Parameters:**
  - `sortBy` (optional): Sort field (default: "name").
  - `ascending` (optional): Sort direction (default: true).
  - `page` (optional): Page number (default: 1).
  - `pageSize` (optional): Page size (default: 10).
- **Responses:**
  - `200 OK`: Returns a list of tags.
  - `500 Internal Server Error`: Indicates an error occurred while fetching tags.

#### Refresh Tags

- **URL:** `/tags/refresh`
- **Method:** `POST`
- **Description:** Refreshes the list of tags by fetching them from the Stack Overflow service.
- **Remarks:** This action clears the existing tags in the repository and retrieves the latest tags from the Stack Overflow service.
- **Responses:**
  - `200 OK`: Indicates successful refresh of tags.
  - `500 Internal Server Error`: Indicates an error occurred while refreshing tags.
