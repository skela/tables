//
//  TablePropertyScanner.m
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//
//  This file contains slightly modified code added by me, but the original was written by Nick Lockwood:
//

//  Original notice:
//
//  Created by Nick Lockwood on 13/02/2014.
//  Copyright (c) 2014 Charcoal Design. All rights reserved.
//
//  Distributed under the permissive zlib License
//  Get the latest version from here:
//
//  https://github.com/nicklockwood/FXForms
//
//  This software is provided 'as-is', without any express or implied
//  warranty.  In no event will the authors be held liable for any damages
//  arising from the use of this software.
//
//  Permission is granted to anyone to use this software for any purpose,
//  including commercial applications, and to alter it and redistribute it
//  freely, subject to the following restrictions:
//
//  1. The origin of this software must not be misrepresented; you must not
//  claim that you wrote the original software. If you use this software
//  in a product, an acknowledgment in the product documentation would be
//  appreciated but is not required.
//
//  2. Altered source versions must be plainly marked as such, and must not be
//  misrepresented as being the original software.
//
//  3. This notice may not be removed or altered from any source distribution.
//

#import "TablePropertyScanner.h"

#import <objc/runtime.h>

NSString *const TablePropertyScannerKey = @"key";
NSString *const TablePropertyScannerKeyType = @"type";
NSString *const TablePropertyScannerKeyClass = @"class";
NSString *const TablePropertyScannerKeyCell = @"cell";
NSString *const TablePropertyScannerKeyTitle = @"title";
NSString *const TablePropertyScannerKeyPlaceholder = @"placeholder";
NSString *const TablePropertyScannerKeyDefaultValue = @"default";
NSString *const TablePropertyScannerKeyOptions = @"options";
NSString *const TablePropertyScannerKeyTemplate = @"template";
NSString *const TablePropertyScannerKeyValueTransformer = @"valueTransformer";
NSString *const TablePropertyScannerKeyAction = @"action";
NSString *const TablePropertyScannerKeySegue = @"segue";
NSString *const TablePropertyScannerKeyHeader = @"header";
NSString *const TablePropertyScannerKeyFooter = @"footer";
NSString *const TablePropertyScannerKeyInline = @"inline";
NSString *const TablePropertyScannerKeySortable = @"sortable";
NSString *const TablePropertyScannerKeyViewController = @"viewController";

NSString *const TablePropertyScannerKeyTypeDefault = @"default";
NSString *const TablePropertyScannerKeyTypeLabel = @"label";
NSString *const TablePropertyScannerKeyTypeText = @"text";
NSString *const TablePropertyScannerKeyTypeLongText = @"longtext";
NSString *const TablePropertyScannerKeyTypeURL = @"url";
NSString *const TablePropertyScannerKeyTypeEmail = @"email";
NSString *const TablePropertyScannerKeyTypePhone = @"phone";
NSString *const TablePropertyScannerKeyTypePassword = @"password";
NSString *const TablePropertyScannerKeyTypeNumber = @"number";
NSString *const TablePropertyScannerKeyTypeInteger = @"integer";
NSString *const TablePropertyScannerKeyTypeUnsigned = @"unsigned";
NSString *const TablePropertyScannerKeyTypeFloat = @"float";
NSString *const TablePropertyScannerKeyTypeBitfield = @"bitfield";
NSString *const TablePropertyScannerKeyTypeBoolean = @"boolean";
NSString *const TablePropertyScannerKeyTypeOption = @"option";
NSString *const TablePropertyScannerKeyTypeDate = @"date";
NSString *const TablePropertyScannerKeyTypeTime = @"time";
NSString *const TablePropertyScannerKeyTypeDateTime = @"datetime";
NSString *const TablePropertyScannerKeyTypeImage = @"image";

@implementation TablePropertyScanner

+ (NSArray *)propertiesForObject:(id)obj
{
    if (!obj)
        return nil;
    
    static void *FXFormPropertiesKey = &FXFormPropertiesKey;
    NSMutableArray *properties = objc_getAssociatedObject(obj, FXFormPropertiesKey);
    if (!properties)
    {
        properties = [NSMutableArray array];
        Class subclass = [obj class];
        while (subclass != [NSObject class])
        {
            unsigned int propertyCount;
            objc_property_t *propertyList = class_copyPropertyList(subclass, &propertyCount);
            for (unsigned int i = 0; i < propertyCount; i++)
            {
                //get property name
                objc_property_t property = propertyList[i];
                const char *propertyName = property_getName(property);
                NSString *key = @(propertyName);
                
                //ignore NSObject properties
                char *readonly = property_copyAttributeValue(property, "R");
                if (readonly)
                {
                    free(readonly);
                    if ([@[@"hash", @"superclass", @"description", @"debugDescription"] containsObject:key])
                    {
                        continue;
                    }
                }
                
                //get property type
                Class valueClass = nil;
                NSString *valueType = nil;
                char *typeEncoding = property_copyAttributeValue(property, "T");
                switch (typeEncoding[0])
                {
                    case '@':
                    {
                        if (strlen(typeEncoding) >= 3)
                        {
                            char *className = strndup(typeEncoding + 2, strlen(typeEncoding) - 3);
                            __autoreleasing NSString *name = @(className);
                            NSRange range = [name rangeOfString:@"<"];
                            if (range.location != NSNotFound)
                            {
                                name = [name substringToIndex:range.location];
                            }
                            valueClass = NSClassFromString(name) ?: [NSObject class];
                            free(className);
                        }
                        break;
                    }
                    case 'c':
                    case 'B':
                    {
                        valueClass = [NSNumber class];
                        valueType = TablePropertyScannerKeyTypeBoolean;
                        break;
                    }
                    case 'i':
                    case 's':
                    case 'l':
                    case 'q':
                    {
                        valueClass = [NSNumber class];
                        valueType = TablePropertyScannerKeyTypeInteger;
                        break;
                    }
                    case 'C':
                    case 'I':
                    case 'S':
                    case 'L':
                    case 'Q':
                    {
                        valueClass = [NSNumber class];
                        valueType = TablePropertyScannerKeyTypeUnsigned;
                        break;
                    }
                    case 'f':
                    case 'd':
                    {
                        valueClass = [NSNumber class];
                        valueType = TablePropertyScannerKeyTypeFloat;
                        break;
                    }
                    case '{': //struct
                    case '(': //union
                    {
                        valueClass = [NSValue class];
                        valueType = TablePropertyScannerKeyTypeLabel;
                        break;
                    }
                    case ':': //selector
                    case '#': //class
                    default:
                    {
                        valueClass = nil;
                        valueType = nil;
                    }
                }
                free(typeEncoding);
                
                //add to properties
                NSMutableDictionary *inferred = [NSMutableDictionary dictionaryWithObject:key forKey:TablePropertyScannerKey];
                if (valueClass) inferred[TablePropertyScannerKeyClass] = valueClass;
                if (valueType) inferred[TablePropertyScannerKeyType] = valueType;
                [properties addObject:[inferred copy]];
            }
            free(propertyList);
            subclass = [subclass superclass];
        }
        objc_setAssociatedObject(obj, FXFormPropertiesKey, properties, OBJC_ASSOCIATION_RETAIN);
    }
    return properties;
}

@end
