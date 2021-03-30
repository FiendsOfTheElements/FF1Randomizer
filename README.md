# FF1 Randomizer

This is a randomizer for Final Fantasy 1 for the NES. It currently operates on the US version of the ROM only. A randomizer takes certain elements of a game and randomize them, creating a whole new gameplay experience.

## Project Structure

The general project structure is as follows:

- UI: FF1Blazorizer
- Actual logic: FF1Lib
- Desktop Application (Obsolete): FF1Randomizer
- Command line FFR: FF1R
- Shared Desktop and Command line stuff: FFR.Common
- deployment scripts: .circleci




## Setting up a Development Environment

1. [Download and install Visual Studio](https://visualstudio.microsoft.com/)
2. In the Visual Studio Installer, select the following optional pieces:

   ASP.NET and web development
   .NET desktop development

3. Clone the randomizer (git specific information below)
4. All done!

### Developing on Linux

1. [Download and install the .NET 5.0 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/linux)
2. `cd FF1Blazorizer && dotnet publish -c Debug -o output`
3. `cd output/wwwroot && python3 -m http.server 8000`

## ROM hacking

### Using ca65 to assemble code

For example, if you need to assemble `0E_9500_ShopUpgrade.asm` and get a hex string (the listing is optional but incredibly helpful):

```
ca65  0E_9500_ShopUpgrade.asm --listing 0E_9500_ShopUpgrade.lst
ld65 -t nes -o 0E_9500_ShopUpgrade 0E_9500_ShopUpgrade.o
hexdump -s 0x10 -v -e '/1 "%02X"' 0E_9500_ShopUpgrade
```

### Hot-patch in the fceux debugger for testing

1. Run the game in fceux, find what you care about in the debugger
1. Click "ROM offsets" to ensure it is on,
1. Open the Hex Editor, then view -> Rom file
1. Ctrl + a to goto an address, input the rom offset
1. Input the assembled hex
1. It will update live

## Git workflow

The randomizer is hosted on github and therefore some level of git knowledge is required in order to contribute. In order to help get anyone ready to contribute, this will assume no prior git knowledge. Please skip past any steps you have already done, or this entire section if you are already familiar with git and able to rebase, merge, deal with multiple repos, bisect, and handle PRs on github.

### Basic Description
#### Git
 A version control system. This will control a folder or directory for you, managing the files inside it so you can swap between versions of those files.

#### Git Terms
 - commit: A discrete piece of history in the version control system.
 - branch: A distinct history of the files in the git directory. This can have a history that shares a common ancestor with another branch (the usual case) or be fully separate. Usually features/bugfixes start with a new branch off the main branch.
 - repo, repository, or remote: A remote server that facilitates distributed development through being a synchronized source of truth for the files in the git directory. There can be multiple repos.
 - rebase or merge: Methods of taking changes that are in a repo and combining them with your local work.
 - pull and push: Transferring files from the repo to your local stuff or the other way around.
 - clone: The action of setting up a local git directory that is a clone of a specific repo.
 - checkout: Swap to a different branch or commit.

 - use `git status` to see what the current situation in your local git directory is.


#### Github
A company that provides git hosting services.

#### Github Terms
   - Fork: A separate repo of a project on github, copied at a specific point in its history. You will likely have a fork of the FFR repo.
   - PR or Pull Request: A way of asking someone to merge your changes from a branch into the main branch. This can be done from a separate fork without issue.
   - Issues: This is a bug/feature system built into github, the FFR project does not make much use of these.


### Getting started

1. Make a github account.
2. Head over to [the git main page](https://git-scm.com/downloads) and install git, there will be instructions for linux and mac here too.
3. Follow the instructions [here](https://docs.github.com/en/github/using-git/setting-your-username-in-git) to set your username in git.
4. Follow the instructions [here](https://docs.github.com/en/github/setting-up-and-managing-your-github-user-account/setting-your-commit-email-address#setting-your-commit-email-address-in-git) to set your email that will be attached to your git commits. You may also want to follow the earlier parts of this page to set your github email.
5. Follow the instructions [here](https://docs.github.com/en/github/getting-started-with-github/set-up-git#next-steps-authenticating-with-github-from-git) to authenticate with Github.
6. Fork the FFR repo. Click on the "Fork" button at the top right, and follow any instructions.
7. Now in your forked repo, clone it. Click on the "Code" button at the top of this page and select your relevant clone type (https or ssh) and copy that url. Then in your terminal/command line, navigate to the directory where you want the FFR directory to reside in, then type: `git clone <url you copied, without the angled brackets here>`
8. Go back to the main FFR repo, [link here](https://github.com/FiendsOfTheElements/FF1Randomizer) and copy the url used to clone it. Add the main FFR repo as a remote using this command: `git remote add <some name you can remember like "main" or "upstream" or whatever> <the url you copied>` -- note that we will be using "upstream" in this documentation, you can sub in your own name you choose.
9. Get yourself a drink and celebrate having setup everything you need to start contributing to FFR!

### Working on a feature

So now you want to work on a new feature or fix a bug! How do you start?

#### First time setup:

1. Run `git fetch --all` this will update your local git with the knowledge of what the remotes have.
2. Run `git checkout upstream/dev --track` this checkout the main development branch of the upstream repo, and track it for changes.

#### Each time you start a feature or bugfix:

1. Run `git checkout -b <some branch name>` this will create a new local branch with any name you choose.
2. Run `git push origin -u HEAD` this will setup your forked repo to have a copy the local branch you just created.
3. Get to work on some feature code!
4. When you have some work done, or are going to stop for a while, run one of the following `git add` commands:

   `git add -u` adds updated files

   `git add -p` interactively adds updated files

   `git add .` adds all the files in the current directory and its sub directories

   `git add -A` adds all files in the git directory and its sub directories

   `git add relative/path/to/file` adds the file specified

5. Then run `git commit` and type in a commit message, this can be anything but try to make it obvious what is contained in that commit by just the message. `git commit -m "some message"` is a shortcut to add small commit messages.


#### Occasionally while working on your feature
Ensure all current work is stored in a commit, so its easy to recover if something bad happens.
- run `git push` - this will update your remote with the commits you have made, and will be a backup incase anything happens.
- run `git fetch upstream` then `git rebase upstream/dev` - this rebase command will find a common history point (when you created this branch) and then take all the commits since then, and place them on top of the current upstream/dev. What this means is git will try to make it like you just wrote all this code after the stuff in upstream/dev, even the new things. You might have conflicts, this is ok. Look at the Merging and Rebasing section below for help.


#### When you are ready for feedback
At this point you should have been talking with people in the dev discord and have something functional that maybe still needs some polish, but is ready for feedback, if not ready to be included in the beta site yet.

1. Rebase, as covered in the section immediately below this one.
2. Run `git push origin`
3. Go to github and head to your fork.
4. Select the branch name you have been working with in the drop down on the middle left at the top.
5. Click the "Pull request" button on the middle right at the top.
6. This should open a window comparing the changes in your branch and the main repo's dev branch.
7. Add a title and write up a description of your changes.
8. If you're not ready to have this merged in, click on the arrow beside the "Create pull request" button, and change this to a draft pull request.
9. If you want feed back from people in particular, select the gear by reviewers on the top right.
10. Once you are satisfied with the title, description, and reviewers, click the create pull request (or draft pull request) button.
11. If you created a draft PR, once you think it is ready to be merged, come back here and change it to a regular PR.
12. Once it is approved and ready, one of the maintainers will merge it in for you. If you find you are waiting a lot, please reach out in the dev discord: we might be gearing up for a release and just holding off, or we might have forgotten, you wont know unless you reach out.
13. Brag about how you contributed to one of the coolest video game projects on github.

#### Merging and Rebasing

When you are working, other people will also be working and we need a way to combine the changes.
We can do this in two ways, _merge_ the changes or _rebase_ your changes. A key difference between merges and rebases is the commit history. In a merge, there will be a _merge commit_, this is a commit that has two parent commits. In a rebase, there will not be any merge commit, instead all the upstream work will be in your branch's history, before your work. Generally try to do a rebase unless the rebase is giving you trouble, then try a merge. Keeping the history linear is not worth a huge hassle.

##### rebasing:
 - have your work commited then run `git fetch upstream`
 - `git rebase upstream/dev` will rebase you on the dev branch of the main repo
 - you will see status updates about applying commits, it might stop before finishing, if this is the case, it will tell you about conflicts. See Resolving Conflicts below.


##### merging:
 - have your work commited then run `git fetch upstream`
 - `git merge upstream/dev` will merge your work with the dev branch of the main repo
 - you might have conflicts to resolve, see Resolving Conflicts below.


##### Resolving Conflicts:
So you have rebased or merged and need to fix a conflict? And what even is a conflict?
Well a conflict is when you change something and someone else changes the same thing, git doesn't know which is correct, or if there needs to be reworking of the code to make stuff work with both pieces.

First you will want to do `git status` - this will show you what is labeled as "both modified". The term "ours" refers to the main branch or the one being rebased onto, and the "theirs" refers to the branch you made changes in, usually. This will be where you have conflicts to resolve. So open up those files and find the areas with "<<<<<<< HEAD". This is the start of a conflict. You need to look at the two pieces of code and fix it up to work as expected with your changes **AND** the changes that were made by others since you started your work.

The first section with be after the "<<<<<<< HEAD" until the "=======" and the second section will be from the "=======" until the ">>>>>>> <some branch name>"

Go through each of these and fix them up, (try searching <<<<<<< HEAD to be sure you got them all), then run `git add -u` followed by:
 - if you did a rebase: `git rebase --continue`, you might have more conflicts as more of your commits are applied.
 - if you did a merge: `git merge --continue`, you will see a commit screen with a pre-filled message, just leave the message and exit out as normal with commits.

If you have issues with resolving a conflict and making stuff work afterwards (either your work or the other work), don't be afraid to reach out in the dev discord for help.

And now your conflict should be resolved!

## Deployment

Our deployments utilize CircleCi and Netlify.
The deployment config can be found in the .circleci folder in the config.yml file. This defines a series of jobs that need to run for our deployment process, first we build with the build.sh file, then we deploy to netlify using the deploy.sh file. The build process modifies a couple values depending on if this is a beta and real release. The deploy process will deploy previews for PRs from maintainers working in the main repo, as well as deploy to either the beta or main site, depending on the branch.

If you are a maintainer working in the main repo, in order to get deployment previews, you need to pass the hold job in circleci: just look in the pinned resources in dev discord to see how to do this.

Beta is updated basically whenever. Never feel bad about updating beta, if the flags break and it ruin's someone's race, its ok.

The main site is updated every month or 3 (or when we get around to it). This *generally* has a 2 week waiting period where beta is frozen, except for bug fixes and the like. If this is the case, your PRs will be sitting there waiting until the release happens.
