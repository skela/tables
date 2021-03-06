//
//  Menu.m
//  tables
//
//  Created by Aleksander Slater on 24/02/2015.
//  Copyright (c) 2015 Davincium. All rights reserved.
//

#import "Menu.h"
#import "TableUtils.h"

@implementation MenuSection

- (instancetype)init:(NSDictionary*)d
{
    self = [super init];
    if (self)
    {
        [self update:d];
    }
    return self;
}

- (instancetype)initWithName:(NSString*)name
{
    self = [super init];
    if (self)
    {
        self.name = name;
    }
    return self;
}

- (void)update:(NSDictionary*)d
{
    self.name = [TableUtils getString:d forKey:@"name" fallback:self.name];
    self.name = [TableUtils getString:d forKey:@"text" fallback:self.name];
    self.cellIdent = [TableUtils getString:d forKey:@"cellIdent" fallback:self.cellIdent];
}

@end

@implementation MenuItem

- (instancetype)init:(NSDictionary*)d
{
    self = [super init];
    if (self)
    {
        [self update:d];
    }
    return self;
}

- (void)update:(NSDictionary*)d
{
    self.cellIdent = [TableUtils getString:d forKey:@"cellIdent" fallback:self.cellIdent];    
    self.text = [TableUtils getString:d forKey:@"text" fallback:self.text];
    self.detail = [TableUtils getString:d forKey:@"detail" fallback:self.detail];
    self.selector = [TableUtils getString:d forKey:@"selector" fallback:self.selector];
    self.deleteSelector = [TableUtils getString:d forKey:@"delete" fallback:self.deleteSelector];
    self.key = [TableUtils getString:d forKey:@"key" fallback:self.key];
    self.cellIdent = [TableUtils getString:d forKey:@"cellIdent" fallback:self.cellIdent];
}

@end
