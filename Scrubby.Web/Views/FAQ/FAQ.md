# Scrubby Web

### General Questions

##### Logging In

Currently Scrubby uses the /tg/station forums for authentication, no sensitive data is sent to Scrubby and we only store
what is necessary to maintain security of the website.

### Usage

##### Rounds

The format: `https://scrubby.melonmesa.com/round/<ROUND_ID_HERE>`

On a round page, once loaded into Scrubby by the scraping server, you can find a list of the players involved in that
round as well as the jobs they held, color coded by department, as well as if they had an antagonist role. Hovering over
one of these names will show the CKey and job of that user, and antagonist role if they had one. If you click on one of
these names, you will be taken to the user's CKey page on Scrubby, where you can view their other names, recently played
rounds, and more information about that user.

If there are any photos associated with the round, they will also be embedded at the bottom of the page.

Under the Files section of the page, you will see each of the files that Scrubby decided to store when downloading the
files from /tg/station's website. When log files are parsed, providing a easily readable format for you, they will be
blue and clickable. If a file fails to be parsed, it will be red. Grey files are unparsed as I have not written an
ingestor for that specific file type yet.

At the top of the page, you will find one or two buttons in the left and right corners which allow you to navigate to
the previous and next round in the sequence.

++Undocumented Features++:

For the rounds page there are a couple undocumented features which are strongly related. The first undocumented feature
allows you to highlight a CKey (user) in a round, which will bold their respective name under Players, this is achieved
by simply adding that user's CKey to the URL, like
this: `https://scrubby.melonmesa.com/round/<ROUND_ID_HERE>?h=<CKEY_HERE>`

Even better, like many features in Scrubby, this supports more than one CKey. You could, for example, highlight multiple
users within a round! (`https://scrubby.melonmesa.com/round/<ROUND_ID_HERE>?h=<CKEY_A>&h=<CKEY_B>`)

Finally, and perhaps the most useful thing about these features, is that the 'previous' and 'next' round buttons are
++relative to the highlighted users++. You could, for example, see all of the rounds that two, three, four, or any
number of users have shared, in chronological order!

##### CKeys

The format: `https://scrubby.melonmesa.com/ckey/<CKEY_HERE>`

When you load a ckey page, you will be shown a graph of the user's most popular in-game names, as well as the count of
rounds they have played in by server. Note that the count shown is rounds that the player *has actively played in* and
does not currently count rounds the user observed or otherwise connected to. This is a planned feature.

At the top of this page, you will see the first round on Scrubby that featured this user, as well as the most recent
rounds they have been in that are on Scrubby. By clicking one of these links, you will automatically have the
ckey-highlighting feature described above applied and will have the user highlighted in the round.

##### Files

The files service is now discontinued as of September 2025.

##### Images

Images are occasionally found within round files, depending on when the logs were generated. When these are available,
you can use the image area of the website to generate resized images (up to 512x512 pixels).

To do this, copy the link of the image (like `https://scrubby.melonmesa.com/image/5ca63dde5585710334e89eec`) and add on
your desired dimensioning. You can specify, X, Y, or both, and the server will resize them automatically.

Ex: `https://scrubby.melonmesa.com/image/5ca63dde5585710334e89eec?x=256&y=128`

##### IC Name Search

Currently this feature, whilst useful, is not as useful as I would like it to be. It will hit on exact matches for
names, so for example "Kashindra" would find "Kashindra Kalcronk", but "Kash" would not. This will soon also have a
regular expression feature to enhance the search considerably, but that will be a feature that is locked behind being
logged in as it is a much more expensive feature comparatively to the existing search.

##### Icon Search

Using the Icon Search page, you can query with a regular expression against the state name (icon name) of every icon in
the /tg/station codebase! The icon states will show you where to find it in the DMI sheets, various pieces of
information about that sprite in particular, rendered images of that sprite, and even animations of it!

# Scrubby API

The Scrubby API has been discontinued as of June 27, 2021. If you were using this service and would still like to have
access to data stored in Scrubby, please contact ``bobbah`` on Discord.