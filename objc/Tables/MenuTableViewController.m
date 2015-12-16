//
//  MenuTableViewController.m
//  tables
//
//  Created by Aleksander Slater on 24/02/2015.
//  Copyright (c) 2015 Davincium. All rights reserved.
//

#import "MenuTableViewController.h"

@implementation MenuTableViewController
@synthesize sections;

- (UIInterfaceOrientationMask)supportedInterfaceOrientations
{
    return UIInterfaceOrientationMaskAll;
}

#pragma mark - Display

- (MenuItem*)itemWithName:(NSString*)name inSection:(NSString*)section
{
    for (MenuSection *sec in sections)
        if ([sec.name isEqualToString:section])
            for (MenuItem *item in sec.items)
                if ([item.text isEqualToString:name])
                    return item;
    return nil;
}

- (MenuItem*)itemWithKey:(NSString*)key inSection:(NSString*)section
{
    for (MenuSection *sec in sections)
        if ([sec.name isEqualToString:section])
            for (MenuItem *item in sec.items)
                if ([item.key isEqualToString:key])
                    return item;
    return nil;
}

- (MenuItem*)itemWithKey:(NSString*)key
{
    for (MenuSection *sec in sections)
        for (MenuItem *item in sec.items)
            if ([item.key isEqualToString:key])
                return item;
    return nil;
}

- (MenuSection*)sectionForItem:(MenuItem*)item
{
    for (MenuSection *sec in sections)
        for (MenuItem *itemInSec in sec.items)
            if (item==itemInSec)
                return sec;
    return nil;
}

- (void)reloadData
{
}

#pragma mark - Table

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section
{
    return [[sections[section] items] count];
}

- (NSString*)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section
{
    return [sections[section] name];
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView
{
    return sections.count;
}

- (BOOL)tableView:(UITableView *)tableView canEditRowAtIndexPath:(NSIndexPath *)indexPath
{
    MenuSection *section = sections[indexPath.section];
    MenuItem *item = section.items[indexPath.row];
    if (item.deleteSelector!=nil)
        return YES;
    return NO;
}

- (BOOL)tableView:(UITableView *)tableView canSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    MenuSection *section = sections[indexPath.section];
    MenuItem *item = section.items[indexPath.row];
    
    if (item.selector!=nil)
    {
        SEL selector = NSSelectorFromString(item.selector);
        if (selector!=NULL && [self respondsToSelector:selector])
        {
            return YES;
        }
    }
    return NO;
}

- (void)tableView:(UITableView *)tableView commitEditingStyle:(UITableViewCellEditingStyle)editingStyle forRowAtIndexPath:(NSIndexPath *)indexPath
{
    MenuSection *section = sections[indexPath.section];
    MenuItem *item = section.items[indexPath.row];
    if (editingStyle==UITableViewCellEditingStyleDelete && item.deleteSelector!=nil)
    {
        SEL selector = NSSelectorFromString(item.deleteSelector);
        if (selector!=NULL && [self respondsToSelector:selector])
        {
            [self performSelectorOnMainThread:selector withObject:item waitUntilDone:YES];
            return;
        }
    }
}

- (UITableViewCell*)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    MenuSection *section = sections[indexPath.section];
    MenuItem *item = section.items[indexPath.row];
    
    NSString *ident = nil;
    if (item.cellIdent!=nil)
        ident = item.cellIdent;
    if (ident==nil && section.cellIdent!=nil)
        ident = section.cellIdent;
    if (ident==nil)
        ident = @"MenuCell";
    
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:ident];
    if (cell==nil)
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleValue1 reuseIdentifier:ident];
    
    if ([cell conformsToProtocol:@protocol(MenuCell)])
    {
        id<MenuCell> mcell = (id<MenuCell>)cell;
        [mcell updateForSection:section andItem:item];
    }
    else
    {
    
        BOOL titleSetter = [cell respondsToSelector:@selector(setTitle:)];
        BOOL detailSetter = [cell respondsToSelector:@selector(setDetail:)];
        
        if (titleSetter || detailSetter)
        {
            if (titleSetter)
                [cell performSelector:@selector(setTitle:) withObject:[item text]];
            if (detailSetter)
                [cell performSelector:@selector(setDetail:) withObject:[item detail]];
        }
        else
        {
            cell.textLabel.text = [item text];
            cell.detailTextLabel.text = [item detail];
        }
        cell.accessoryType = [self tableView:tableView canSelectRowAtIndexPath:indexPath] ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
    }
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    if (![self tableView:tableView canSelectRowAtIndexPath:indexPath])
        return;
    
    MenuSection *section = sections[indexPath.section];
    MenuItem *item = section.items[indexPath.row];
    
    if (item.selector!=nil)
    {
        SEL selector = NSSelectorFromString(item.selector);
        if (selector!=NULL && [self respondsToSelector:selector])
        {
            [self performSelectorOnMainThread:selector withObject:item waitUntilDone:YES];
            return;
        }
    }
}

@end
