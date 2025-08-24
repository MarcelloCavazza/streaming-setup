#!/bin/bash

deleteFolders() {
    rm -rf bin/
    rm -rf obj/
}

cd client && deleteFolders
cd ../innerservice && deleteFolders
cd ../outerservice && deleteFolders

cd ..

dotnet build