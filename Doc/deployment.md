# FFR versioning and deployment

## Requirements

* Links that get shared among users should be permanent links, that
  means the version of the site pointed to by the link doesn't change.
  This makes it possible to have links with flagsets and seeds remain
  valid over time.  This includes both the stable and beta versions.
* It should be possible to install the site for offline use as a
  "progressive web app" (PWA)
* As much as possible, users should have access to the same
  preferences, cached ROM, and presets among different versions

## Instanced sites

Sites are deployed to separate domains under
finalfantasyrandomizer.com.  The version number is first part of the
domain.  For stable releases, this is the version number with dots
replaced with dashes, for example, release 4.1.6 is deployed to
4-1-6.finalfantasyrandomizer.com.  Beta sites are deployed as
beta-[first 8 characters of the git commit hash].

The front pages of finalfantasyrandomizer.com and
beta.finalfantasyrandomizer.com are a redirect page that sends the
user to the latest version.

## Offline installation

Users can install FFR to their local browser.  Installation means
installing the service worker, the code for this is at
wwwroot/service-worker.published.js.  The service worker instructs the
browser to cache all the assets related to the site.

Because each version is its own sites, users install each version
separately.

## Shared storage

Preferences, presets and the base ROM are shared between versions by this strategy:

1. The instanced site includes an iframe from finalfantasyrandomizer.com
2. The instanced site posts a messages to that iframe
3. The javascript code inside the iframe receives the message
4. The javascript code inside the iframe saves/loads localStorage,
  which will be the localStorage for finalfantasyrandomizer.com
  instead of the instanced site.
5. The javascript code inside the iframe posts the response to the parent
6. The instanced site receives the response and loads the value.

?how to store preferences for PWAs?

## Forward/backwards compatability for preferences

Sharing preferences and presets means it will be accessed from
different versions of the site.  This is handled the following ways.

For preferences, whatever preferences are compatible with the current
version are loaded.  Unrecognized preferences are ignored.
Incompatible preferences will be set to their default value for that
version.

When saving preferences, we first load the existing preferences, then
copy the new preferences on top of them.  This means keys that were
set by a different version and unrecognized by this version are left
alone.

## Forward/backwards compatability for presets

For presets, currently the storage format is extremely simple, just
json object of preset name -> string, where the string is a
stringified flags json.

Presets which the user does not interact with are not parsed, so
presets from different versions can easily coexist.

When loading a preset, unrecognized, incompatible or missing entries
produce a warning and are set to their default values.  Loading a
preset does not modify the presets in storage unless it is saved
again.

Saving a preset adds/replaces the preset entry with the preset name
and stringified json flags from the current version, and saves the
presets.  It does not modify any of the other presets.

Deleting a preset removes the key and rewrites the presets.  It does
not modify any of the other presets.
