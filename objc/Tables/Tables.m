//
//  Tables.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "Tables.h"



@implementation TestData
@synthesize version;

- (id)initWithVersion:(NSString*)aVersion
{
    self = [super init];
    if (self)
    {
        version = aVersion;
    }
    return self;
}

+ (TestData*)createTestData
{
    TestData *data = [[TestData alloc] initWithVersion:@"1.0.0"];
    data.name = @"Bob";
    data.chosen = NO;
    data.build = @"2";
    data.telephone = @"1337";
    data.stuff = @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
    data.cool = YES;
    NSDateFormatter *f = [[NSDateFormatter alloc] init];
    [f setDateFormat:@"yyyy,MM,dd,hh,mm,ss"];
    data.time1 = [f dateFromString:@"2013,3,14,13,37,00"];
    data.time2 = [f dateFromString:@"2013,3,14,13,37,00"];
    data.time3 = [f dateFromString:@"2013,3,14,13,37,00"];
    data.singleChoice = TestDataItemOptionPizza;
    return data;
}

@end