# Experimenting with Git Tag

so git tagging is basically naming a unique commit that isn't viewed by the git repo as a standard commit of code.  What I mean by this is that you can almost see it as a separate branch within your branch.

Let's play with the common commands required.

### Creating a Tag

Once you have your code ready for a release.  Commit your code as you normally would.  You can push, or not, it's up to you at this point.

Now we need to make a tag as this commit is going to mark a particular version that is going to be deployed into production or released as an update.

To do so, run the following command:

```bash
git tag -a 1.4a -m "This is the release message"
```

Note: this version is marking it as an alpha release, and the message is required, but if you don't put anything, it's like a commit message in that it will bring up vim.

Now that you've made your tag, you need to push the code with a regular git push and push the tag to your remote repo as a standard git push doesn't do that.

To push your git tag, run the following command:

```bash
git push origin 1.4a
```

Note: the version number that you put at the end of the command needs to be the same as the tag that you're looking to push up to the remote repNote: the version number that you put at the end of the command needs to be the same as the tag that you're looking to push up to the remote repo


### Checking out a Tag

To check out a tag, simply run the following command:

```bash
git checkout 1.4a
```

Then to go back, run this:

```bash
git switch -
```

Now you can make changes to and update a tag, however, I don't wanna do that or really see much of a point unless you're doing a security update to a version of your codebase that is still under active support.



