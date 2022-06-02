**GitLFS Quota Reached! Wait until 6/13/2022 for final update to be uploaded when quota resets**

Refer to x64 debug build.

upload .dll files with GitLFS

Repository setup:

Open command prompt in the hmm/HMM directory

Git for Windows: https://gitforwindows.org/

After installing Git, to download the repo locally: git clone https://github.com/SlashRobotics/Honeysuckle-Meadery

To update the repository install Git LFS: git lfs install

Add the file extensions for large files over 100mb to track: git lfs track "*.dll"

Add all files to the history: git add *

Commit the files: git commit -m "first commit"

Get the branch: git branch -M main

Run: git remote add origin https://github.com/SlashRobotics/Honeysuckle-Meadery.git (First time only)

Finally push the files to the repository on the main branch: git push -u origin main

You should see an "Uploading LFS objects" first then the other files will follow.

You may have to pull: git pull -all, or reset: git reset/git reset --hard (removes history), to make a push.
