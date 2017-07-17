# Usage-Data-Anonymiser
Windows console app for anonymising usage data for the Microsoft Recommendations API.

The Microsoft Cognitive Services Recommendations API takes a CSV file containing usage events in the following format:

`<UserId>,<ItemId>,<Time>`

This application will replace each user id with a Guid so that user data is not shared.

Kudos to [Martin Beeby](https://github.com/thebeebs) for helping with the logic and performance.
