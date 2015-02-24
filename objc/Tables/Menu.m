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
    self.text = [TableUtils getString:d forKey:@"text" fallback:self.text];
    self.detail = [TableUtils getString:d forKey:@"detail" fallback:self.detail];
    self.selector = [TableUtils getString:d forKey:@"selector" fallback:self.selector];
    self.deleteSelector = [TableUtils getString:d forKey:@"delete" fallback:self.deleteSelector];
    self.key = [TableUtils getString:d forKey:@"key" fallback:self.key];
}

@end
