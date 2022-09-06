# wpsequencer

Firefox extension addon with C# native messaging app automates querying, saving phone numbers into a database

This firefox addon with a C# native messaging app 
operates a website about like a human being would.

It overcomes human-action obstacles coded in a web site.

Know the players:
"CS" = Javascript Firefox Addon Content Script
"BS" = Javascript Firefox Addon Background Script
"C#" = Windows C# Firefox Native Messaging App
"DOM" = Document Object Model, of some webpage.

What an awesome new day dawning for scraping web pages:
The CS has a period timer to examine current situation,
just like a human looking at page, and to take actions!

1. Have a state variable describing the steps in the sequence.
2. Periodically examine the DOM for some condition to proceed.
3. When needed, send a query to C# for the next query strings.

The C# app starts as soon as the addon is installed in Firefox.
User, me, navigates to website, and clicks addon button to start.
The BS uploads and executes CS to work DOM of matching website.
The CS loops, examining DOM, recognizing things, takes actions.
Henceforth, BS just relays JSON packets between the CS and C#.
The C# app exchanges JSON packets with Firefox addon CS via BS.
The C# app Day1 MVP had a GUI with two buttons and text in/out.
The buttons can kick process if CS displays a certain warning.
The C# app accepts JSON packet commands, sends JSON responses.
The C# app queries MySQL database, then updates phone numbers.

There is an alternate CS that can just scrape and format any DOM,
which the C# app saves to a text file for inspection and planning.
