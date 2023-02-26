import os
import requests
import json
from getpass import getpass
from dotenv import load_dotenv

load_dotenv()

SERVER_URL = os.environ['SERVER_URL']
VERIFY = True

# SERVER_URL = os.environ['LOCAL_SERVER_URL']
# VERIFY = False

DOCUMENTS_DIR = os.environ['DOCUMENTS_DIR']


def login() -> str:
    username = input('Username: ')
    password = getpass('Password: ')
    response = requests.post(f'{ SERVER_URL }/api/Auth/login', json={ 'userName': username, 'password': password }, verify=VERIFY)

    if response.status_code != 200:
        raise Exception('[ERROR] Authentication failed')

    os.system('cls')
    json_response = json.loads(response.content)
    jwt = json_response['jwt']
    return jwt


def get_semester() -> str:
    semester = input('Enter Semester: ')

    valid_options = [str(i) for i in range(1, 11)]
    valid_options.append('*')
    if semester not in valid_options:
        raise Exception('[ERROR] Invalid Semester')

    return '*' if semester == '*' else f'Semester_{ semester }'


def get_subject(semester: str) -> str:
    subject = input('Enter Subject: ')
    
    valid_options = get_all_subjects(semester)
    valid_options.append('*')
    if subject not in valid_options:
        raise Exception('[Error] Invalid Subject')
    os.system('cls')

    return subject
    
    
def get_all_semesters() -> list:
    semester_dirs = os.listdir(DOCUMENTS_DIR)
    return semester_dirs


def get_all_subjects(semester: str) -> list:
    subject_dirs = os.listdir(f'{ DOCUMENTS_DIR }\\{ semester }')
    return subject_dirs


def load_config(semester: str, subject: str) -> dict:
    try:
        with open(f'{ DOCUMENTS_DIR }\\{ semester }\\{ subject }\\config.json', 'r') as file:
            content = file.read()
            return json.loads(content)
    except FileNotFoundError:
        print(f'[ERROR] { semester } { subject }: Config file not found')


def add_summary(jwt: str, semester: int, subject: str, name: str, keywords: list) -> int:
    url = f'{ SERVER_URL }/api/Summary'
    headers = { 'Authorization': f'Bearer { jwt }' }
    json = {
        'semester': semester,
        'subject': subject,
        'name': name,
        'keywords': keywords
    }
    response = requests.post(url, headers=headers, json=json, verify=VERIFY)

    if response.status_code != 200:
        raise Exception('[ERROR] Summary upload failed')
    
    return int(response.content)


def generate_image_path(semester: str, subject: str, idx: int) -> str:
    file_name = f"{ subject }_{ f'0{ str(idx) }' if idx < 10 else str(idx)}.jpg"
    image_path = f'{ DOCUMENTS_DIR }\\{ semester }\\{ subject }\\{ file_name }'

    return image_path


def upload_image(jwt: str, summary_id: int, idx: int, semester: str, subject: str):
    url = f'{ SERVER_URL }/api/Summary/upload/{ summary_id }'
    headers = { 'Authorization': f'Bearer { jwt }' }

    file_path = generate_image_path(semester, subject, idx + 1)
    files = { 'image': ('image.jpg', open(file_path, 'rb'), 'image/jpeg') }

    response = requests.post(url, headers=headers, files=files, verify=VERIFY)

    if response.status_code != 200:
        raise Exception('[Error] Image upload failed')


def post_subject(jwt: str, semester: str, subject: str):
    config = load_config(semester, subject)
    
    for idx, summary in enumerate(config['summaries']):
        print(semester, config['subjectname'], summary['name'])
        summary_id = add_summary(jwt, int(semester.split('_')[1]), config['subjectname'], summary['name'], summary['keywords'])
        upload_image(jwt, summary_id, idx, semester, subject)
    

def main():
    try:
        jwt = login()
        semester = get_semester()
        if semester == '*':
            for semester in get_all_semesters():
                for subject in get_all_subjects(semester):
                    post_subject(jwt, semester, subject)
        else:
            subject = get_subject(semester)
            if subject == '*':
                for subject in get_all_subjects(semester):
                    post_subject(jwt, semester, subject)
            else:
                post_subject(jwt, semester, subject)

    except Exception as ex:
        print(str(ex))


if __name__ == '__main__':
    main()
