//
//  AppDelegate.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>
#import "Tables.h"

@interface AppDelegate : UIResponder <UIApplicationDelegate>
@property (strong, nonatomic) UIWindow *window;
@end

@interface TableViewController : UIViewController
@property (strong, nonatomic) TableAdapter *adapter;
@end
