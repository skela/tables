//
//  Menu.h
//  tables
//
//  Created by Aleksander Slater on 24/02/2015.
//  Copyright (c) 2015 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface MenuSection : NSObject
@property(nonatomic,strong) NSArray *items;
@property(nonatomic,copy) NSString *name;
@property(nonatomic,strong) NSObject *object;
@end

@interface MenuItem : NSObject
@property(nonatomic,copy) NSString *text;
@property(nonatomic,copy) NSString *detail;
@property(nonatomic,copy) NSString *key;
@property(nonatomic,copy) NSString *selector;
@property(nonatomic,copy) NSString *deleteSelector;
@property(nonatomic,strong) NSObject *object;
@property(nonatomic,readwrite) BOOL checked;
@property(nonatomic,readwrite) NSInteger badge;

- (instancetype)init:(NSDictionary*)d;
- (void)update:(NSDictionary*)d;

@end

