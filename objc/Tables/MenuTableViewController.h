//
//  MenuTableViewController.h
//  tables
//
//  Created by Aleksander Slater on 24/02/2015.
//  Copyright (c) 2015 Davincium. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "Menu.h"

@protocol  MenuCell<NSObject>
- (void)updateForSection:(MenuSection*)section andItem:(MenuItem*)item;
@end

@interface MenuTableViewController : UITableViewController

@property(nonatomic,strong) NSArray *sections;

- (MenuItem*)itemWithName:(NSString*)name inSection:(NSString*)section;
- (MenuItem*)itemWithKey:(NSString*)key inSection:(NSString*)section;
- (MenuItem*)itemWithKey:(NSString*)key;
- (MenuSection*)sectionForItem:(MenuItem*)item;
- (void)reloadData;
- (BOOL)tableView:(UITableView *)tableView canSelectRowAtIndexPath:(NSIndexPath *)indexPath;

@end
