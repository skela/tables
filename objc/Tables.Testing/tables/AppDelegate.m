//
//  AppDelegate.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "AppDelegate.h"

@implementation AppDelegate

- (BOOL)application:(UIApplication *)application didFinishLaunchingWithOptions:(NSDictionary *)launchOptions
{
    self.window = [[UIWindow alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
    [self.window setBackgroundColor:[UIColor whiteColor]];
    [self.window setRootViewController:[TableViewController new]];
    [self.window makeKeyAndVisible];
    return YES;
}

@end

@implementation TableViewController

- (void)viewDidLoad
{
    [super viewDidLoad];
    
    id data = [TestData createTestData];
    
    UITableView *tv = [[UITableView alloc] initWithFrame:self.view.bounds];
    tv.autoresizingMask = UIViewAutoresizingFlexibleHeight | UIViewAutoresizingFlexibleWidth;
    self.adapter = [[TableAdapter alloc] initWithTable:tv andData:data];
    [self.view addSubview:tv];
}

@end